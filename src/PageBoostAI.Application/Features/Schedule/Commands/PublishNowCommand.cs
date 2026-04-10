using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record PublishNowCommand(Guid UserId, Guid ScheduleId) : IRequest<Result<ScheduleDto>>;

public class PublishNowCommandHandler : IRequestHandler<PublishNowCommand, Result<ScheduleDto>>
{
    private readonly IContentScheduleRepository _contentScheduleRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly IEncryptionService _encryptionService;
    private readonly IUsageMetricsRepository _usageMetricsRepository;

    public PublishNowCommandHandler(
        IContentScheduleRepository contentScheduleRepository,
        IFacebookPageRepository facebookPageRepository,
        IFacebookGraphService facebookGraphService,
        IEncryptionService encryptionService,
        IUsageMetricsRepository usageMetricsRepository)
    {
        _contentScheduleRepository = contentScheduleRepository;
        _facebookPageRepository = facebookPageRepository;
        _facebookGraphService = facebookGraphService;
        _encryptionService = encryptionService;
        _usageMetricsRepository = usageMetricsRepository;
    }

    public async Task<Result<ScheduleDto>> Handle(PublishNowCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _contentScheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        var page = await _facebookPageRepository.GetByIdAsync(schedule.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        string facebookPostId;
        try
        {
            var accessToken = _encryptionService.Decrypt(page.PageAccessToken);
            facebookPostId = await _facebookGraphService.PublishPostAsync(
                accessToken, page.FacebookPageId, schedule.Content.Text, schedule.ImageUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            schedule.MarkFailed(ex.Message);
            await _contentScheduleRepository.UpdateAsync(schedule, cancellationToken);
            return Result<ScheduleDto>.Failure($"Failed to publish to Facebook: {ex.Message}");
        }

        schedule.Publish(facebookPostId);
        await _contentScheduleRepository.UpdateAsync(schedule, cancellationToken);

        var metrics = await _usageMetricsRepository.GetOrCreateCurrentMonthAsync(request.UserId, cancellationToken);
        metrics.IncrementPublished();
        await _usageMetricsRepository.UpdateAsync(metrics, cancellationToken);

        return Result<ScheduleDto>.Success(
            new ScheduleDto(schedule.Id, schedule.PageId, page.PageName, schedule.Content.Text,
                schedule.Status.ToString(), schedule.ScheduledFor, schedule.PublishedAt,
                schedule.ImageUrl, schedule.Hashtags, schedule.CallToAction, schedule.FacebookPostId, schedule.CreatedAt));
    }
}
