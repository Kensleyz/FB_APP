using Microsoft.Extensions.Logging;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Application.Common.Interfaces;

namespace PageBoostAI.Infrastructure.BackgroundJobs;

public interface IPostPublishingJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public class PostPublishingJob : IPostPublishingJob
{
    private const int MaxRetries = 3;

    private readonly IContentScheduleRepository _contentScheduleRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IUsageMetricsRepository _usageMetricsRepository;
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly ILogger<PostPublishingJob> _logger;

    public PostPublishingJob(
        IContentScheduleRepository contentScheduleRepository,
        IFacebookPageRepository facebookPageRepository,
        IUsageMetricsRepository usageMetricsRepository,
        IFacebookGraphService facebookGraphService,
        ILogger<PostPublishingJob> logger)
    {
        _contentScheduleRepository = contentScheduleRepository;
        _facebookPageRepository = facebookPageRepository;
        _usageMetricsRepository = usageMetricsRepository;
        _facebookGraphService = facebookGraphService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting post publishing job");

        var scheduledPosts = await _contentScheduleRepository.GetScheduledPostsAsync(DateTime.UtcNow, cancellationToken);

        _logger.LogInformation("Found {Count} posts to publish", scheduledPosts.Count);

        foreach (var post in scheduledPosts)
        {
            try
            {
                var page = await _facebookPageRepository.GetByIdAsync(post.PageId, cancellationToken);
                if (page is null || !page.IsActive)
                {
                    post.MarkFailed("Associated Facebook page not found or inactive.");
                    await _contentScheduleRepository.UpdateAsync(post, cancellationToken);
                    continue;
                }

                var message = post.Content.Text;
                if (post.Hashtags.Count > 0)
                    message += "\n\n" + string.Join(" ", post.Hashtags.Select(h => h.StartsWith('#') ? h : $"#{h}"));
                if (post.CallToAction is not null)
                    message += "\n\n" + post.CallToAction;

                var facebookPostId = await _facebookGraphService.PublishPostAsync(
                    page.PageAccessToken,
                    page.FacebookPageId,
                    message,
                    post.ImageUrl,
                    cancellationToken);

                post.Publish(facebookPostId);
                await _contentScheduleRepository.UpdateAsync(post, cancellationToken);

                var metrics = await _usageMetricsRepository.GetOrCreateCurrentMonthAsync(page.UserId, cancellationToken);
                metrics.IncrementPublished();
                await _usageMetricsRepository.UpdateAsync(metrics, cancellationToken);

                _logger.LogInformation("Published post {PostId} to Facebook as {FacebookPostId}", post.Id, facebookPostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish post {PostId}", post.Id);

                if (post.RetryCount < MaxRetries)
                {
                    post.IncrementRetry();
                }
                else
                {
                    post.MarkFailed($"Failed after {MaxRetries} retries: {ex.Message}");
                }

                await _contentScheduleRepository.UpdateAsync(post, cancellationToken);
            }
        }

        _logger.LogInformation("Post publishing job completed");
    }
}
