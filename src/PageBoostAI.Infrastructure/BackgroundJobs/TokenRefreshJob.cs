using Microsoft.Extensions.Logging;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Infrastructure.ExternalServices;

namespace PageBoostAI.Infrastructure.BackgroundJobs;

public interface ITokenRefreshJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public class TokenRefreshJob : ITokenRefreshJob
{
    private static readonly TimeSpan TokenExpiryThreshold = TimeSpan.FromDays(7);

    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly ILogger<TokenRefreshJob> _logger;

    public TokenRefreshJob(
        IFacebookPageRepository facebookPageRepository,
        IFacebookGraphService facebookGraphService,
        ILogger<TokenRefreshJob> logger)
    {
        _facebookPageRepository = facebookPageRepository;
        _facebookGraphService = facebookGraphService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting token refresh job");

        var allPages = await _facebookPageRepository.GetAllAsync(cancellationToken);
        var expiringPages = allPages
            .Where(p => p.IsActive && p.AccessTokenExpiresAt.HasValue
                        && p.AccessTokenExpiresAt.Value <= DateTime.UtcNow.Add(TokenExpiryThreshold))
            .ToList();

        _logger.LogInformation("Found {Count} pages with tokens expiring within {Days} days",
            expiringPages.Count, TokenExpiryThreshold.Days);

        foreach (var page in expiringPages)
        {
            try
            {
                var pageInfo = await _facebookGraphService.GetPageInfoAsync(
                    page.PageAccessToken, page.FacebookPageId, cancellationToken);

                if (pageInfo.AccessToken is not null)
                {
                    // Long-lived tokens typically expire in ~60 days
                    page.RefreshToken(pageInfo.AccessToken, DateTime.UtcNow.AddDays(60));
                    await _facebookPageRepository.UpdateAsync(page, cancellationToken);

                    _logger.LogInformation("Refreshed token for page {PageId} ({PageName})",
                        page.FacebookPageId, page.PageName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh token for page {PageId} ({PageName})",
                    page.FacebookPageId, page.PageName);
            }
        }

        _logger.LogInformation("Token refresh job completed");
    }
}
