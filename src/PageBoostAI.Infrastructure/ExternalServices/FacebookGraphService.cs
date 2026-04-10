using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PageBoostAI.Application.Common.Interfaces;

namespace PageBoostAI.Infrastructure.ExternalServices;

public class FacebookGraphService : IFacebookGraphService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FacebookGraphService> _logger;
    private readonly string _appId;
    private readonly string _appSecret;
    private readonly string _redirectUri;

    public FacebookGraphService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FacebookGraphService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _appId = configuration["FACEBOOK_APP_ID"] ?? configuration["FacebookSettings:AppId"] ?? string.Empty;
        _appSecret = configuration["FACEBOOK_APP_SECRET"] ?? configuration["FacebookSettings:AppSecret"] ?? string.Empty;
        _redirectUri = configuration["FACEBOOK_REDIRECT_URI"] ?? configuration["FacebookSettings:RedirectUri"] ?? string.Empty;

        _httpClient.BaseAddress = new Uri("https://graph.facebook.com/v19.0/");
    }

    public async Task<List<FacebookPageInfo>> GetPagesAsync(string userAccessToken, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"me/accounts?fields=id,name,category,access_token,followers_count,picture&access_token={userAccessToken}",
            cancellationToken);

        var pages = new List<FacebookPageInfo>();
        foreach (var page in response.GetProperty("data").EnumerateArray())
        {
            pages.Add(new FacebookPageInfo(
                PageId: page.GetProperty("id").GetString()!,
                PageName: page.GetProperty("name").GetString()!,
                AccessToken: page.GetProperty("access_token").GetString()!,
                Category: page.TryGetProperty("category", out var cat) ? cat.GetString() : null,
                ProfilePictureUrl: page.TryGetProperty("picture", out var pic)
                    ? pic.GetProperty("data").GetProperty("url").GetString()
                    : null));
        }

        _logger.LogInformation("Retrieved {Count} pages for user", pages.Count);
        return pages;
    }

    public async Task<string> PublishPostAsync(string pageAccessToken, string pageId, string message, string? imageUrl = null, CancellationToken cancellationToken = default)
    {
        var endpoint = imageUrl is not null ? $"{pageId}/photos" : $"{pageId}/feed";
        var content = new Dictionary<string, string>
        {
            ["access_token"] = pageAccessToken,
            ["message"] = message
        };
        if (imageUrl is not null)
            content["url"] = imageUrl;

        var response = await _httpClient.PostAsync(endpoint, new FormUrlEncodedContent(content), cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var postId = json.GetProperty("id").GetString()!;

        _logger.LogInformation("Published post {PostId} to page {PageId}", postId, pageId);
        return postId;
    }

    public async Task<FacebookInsights> GetInsightsAsync(string pageAccessToken, string pageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(
                $"{pageId}?fields=fan_count,talking_about_count&access_token={pageAccessToken}",
                cancellationToken);

            var followers = response.TryGetProperty("fan_count", out var fc) ? fc.GetInt32() : 0;
            var talking = response.TryGetProperty("talking_about_count", out var ta) ? ta.GetInt32() : 0;
            var engagementRate = followers > 0 ? (double)talking / followers * 100 : 0;

            return new FacebookInsights(followers, engagementRate, talking, talking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get insights for page {PageId}", pageId);
            return new FacebookInsights(0, 0, 0, 0);
        }
    }

    public async Task<string> ExchangeCodeForTokenAsync(string authCode, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"oauth/access_token?client_id={_appId}&client_secret={_appSecret}&code={authCode}&redirect_uri={Uri.EscapeDataString(_redirectUri)}",
            cancellationToken);

        return response.GetProperty("access_token").GetString()!;
    }

    public async Task<(string Token, DateTime ExpiresAt)> GetLongLivedTokenAsync(string shortLivedToken, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"oauth/access_token?grant_type=fb_exchange_token&client_id={_appId}&client_secret={_appSecret}&fb_exchange_token={shortLivedToken}",
            cancellationToken);

        var token = response.GetProperty("access_token").GetString()!;
        var expiresIn = response.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 5184000;

        return (token, DateTime.UtcNow.AddSeconds(expiresIn));
    }
}
