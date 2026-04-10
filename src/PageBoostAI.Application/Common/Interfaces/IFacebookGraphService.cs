namespace PageBoostAI.Application.Common.Interfaces;

public record FacebookPageInfo(string PageId, string PageName, string AccessToken, string? Category, string? ProfilePictureUrl);
public record FacebookInsights(int Followers, double EngagementRate, int Reach, int Impressions);

public interface IFacebookGraphService
{
    Task<List<FacebookPageInfo>> GetPagesAsync(string userAccessToken, CancellationToken cancellationToken = default);
    Task<string> PublishPostAsync(string pageAccessToken, string pageId, string message, string? imageUrl = null, CancellationToken cancellationToken = default);
    Task<FacebookInsights> GetInsightsAsync(string pageAccessToken, string pageId, CancellationToken cancellationToken = default);
    Task<string> ExchangeCodeForTokenAsync(string authCode, CancellationToken cancellationToken = default);
    Task<(string Token, DateTime ExpiresAt)> GetLongLivedTokenAsync(string shortLivedToken, CancellationToken cancellationToken = default);
    string BuildAuthUrl(string state);
}
