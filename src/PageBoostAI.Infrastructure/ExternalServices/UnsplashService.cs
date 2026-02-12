using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PageBoostAI.Infrastructure.ExternalServices;

public interface IUnsplashService
{
    Task<UnsplashImage?> SearchImageAsync(string query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnsplashImage>> SearchImagesAsync(string query, int count = 5, CancellationToken cancellationToken = default);
}

public record UnsplashImage(string Id, string Url, string SmallUrl, string ThumbUrl, string? Description, string? AuthorName);

public class UnsplashService : IUnsplashService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UnsplashService> _logger;

    public UnsplashService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<UnsplashService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.unsplash.com/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {configuration["UNSPLASH_ACCESS_KEY"]}");
    }

    public async Task<UnsplashImage?> SearchImageAsync(string query, CancellationToken cancellationToken = default)
    {
        var results = await SearchImagesAsync(query, 1, cancellationToken);
        return results.FirstOrDefault();
    }

    public async Task<IReadOnlyList<UnsplashImage>> SearchImagesAsync(string query, int count = 5, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"search/photos?query={Uri.EscapeDataString(query)}&per_page={count}&orientation=landscape",
            cancellationToken);

        var images = new List<UnsplashImage>();
        foreach (var result in response.GetProperty("results").EnumerateArray())
        {
            var urls = result.GetProperty("urls");
            var user = result.GetProperty("user");

            images.Add(new UnsplashImage(
                Id: result.GetProperty("id").GetString()!,
                Url: urls.GetProperty("regular").GetString()!,
                SmallUrl: urls.GetProperty("small").GetString()!,
                ThumbUrl: urls.GetProperty("thumb").GetString()!,
                Description: result.TryGetProperty("description", out var desc) ? desc.GetString() : null,
                AuthorName: user.GetProperty("name").GetString()));
        }

        _logger.LogInformation("Searched Unsplash for '{Query}', found {Count} images", query, images.Count);

        return images;
    }
}
