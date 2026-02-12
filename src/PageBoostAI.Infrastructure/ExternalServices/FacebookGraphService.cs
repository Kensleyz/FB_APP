using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PageBoostAI.Infrastructure.ExternalServices;

public interface IFacebookGraphService
{
    Task<string> PublishPostAsync(string pageAccessToken, string pageId, string message, string? imageUrl = null, CancellationToken cancellationToken = default);
    Task<FacebookPageInfo> GetPageInfoAsync(string pageAccessToken, string pageId, CancellationToken cancellationToken = default);
    Task<string> ExchangeCodeForTokenAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FacebookPageInfo>> GetUserPagesAsync(string userAccessToken, CancellationToken cancellationToken = default);
}

public record FacebookPageInfo(string Id, string Name, string? Category, string? AccessToken, int FollowerCount, string? ProfilePictureUrl);

public class FacebookGraphService : IFacebookGraphService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FacebookGraphService> _logger;
    private readonly string _appId;
    private readonly string _appSecret;

    public FacebookGraphService(
        HttpClient httpClient,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        ILogger<FacebookGraphService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _appId = configuration["FACEBOOK_APP_ID"] ?? string.Empty;
        _appSecret = configuration["FACEBOOK_APP_SECRET"] ?? string.Empty;

        _httpClient.BaseAddress = new Uri("https://graph.facebook.com/v19.0/");
    }

    public async Task<string> PublishPostAsync(string pageAccessToken, string pageId, string message, string? imageUrl = null, CancellationToken cancellationToken = default)
    {
        var endpoint = imageUrl is not null
            ? $"{pageId}/photos"
            : $"{pageId}/feed";

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

    public async Task<FacebookPageInfo> GetPageInfoAsync(string pageAccessToken, string pageId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"{pageId}?fields=id,name,category,followers_count,picture&access_token={pageAccessToken}",
            cancellationToken);

        return new FacebookPageInfo(
            Id: response.GetProperty("id").GetString()!,
            Name: response.GetProperty("name").GetString()!,
            Category: response.TryGetProperty("category", out var cat) ? cat.GetString() : null,
            AccessToken: pageAccessToken,
            FollowerCount: response.TryGetProperty("followers_count", out var fc) ? fc.GetInt32() : 0,
            ProfilePictureUrl: response.TryGetProperty("picture", out var pic)
                ? pic.GetProperty("data").GetProperty("url").GetString()
                : null);
    }

    public async Task<string> ExchangeCodeForTokenAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"oauth/access_token?client_id={_appId}&client_secret={_appSecret}&code={code}&redirect_uri={Uri.EscapeDataString(redirectUri)}",
            cancellationToken);

        return response.GetProperty("access_token").GetString()!;
    }

    public async Task<IReadOnlyList<FacebookPageInfo>> GetUserPagesAsync(string userAccessToken, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"me/accounts?fields=id,name,category,access_token,followers_count,picture&access_token={userAccessToken}",
            cancellationToken);

        var pages = new List<FacebookPageInfo>();
        foreach (var page in response.GetProperty("data").EnumerateArray())
        {
            pages.Add(new FacebookPageInfo(
                Id: page.GetProperty("id").GetString()!,
                Name: page.GetProperty("name").GetString()!,
                Category: page.TryGetProperty("category", out var cat) ? cat.GetString() : null,
                AccessToken: page.GetProperty("access_token").GetString(),
                FollowerCount: page.TryGetProperty("followers_count", out var fc) ? fc.GetInt32() : 0,
                ProfilePictureUrl: page.TryGetProperty("picture", out var pic)
                    ? pic.GetProperty("data").GetProperty("url").GetString()
                    : null));
        }

        _logger.LogInformation("Retrieved {Count} pages for user", pages.Count);

        return pages;
    }
}
