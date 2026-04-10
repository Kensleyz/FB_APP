using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PageBoostAI.Application.Common.Interfaces;

namespace PageBoostAI.Infrastructure.ExternalServices;

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

    public async Task<List<UnsplashPhoto>> SearchPhotosAsync(string query, int count = 10, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"search/photos?query={Uri.EscapeDataString(query)}&per_page={count}&orientation=landscape",
            cancellationToken);

        var photos = new List<UnsplashPhoto>();
        foreach (var result in response.GetProperty("results").EnumerateArray())
        {
            var urls = result.GetProperty("urls");
            var user = result.GetProperty("user");

            photos.Add(new UnsplashPhoto(
                Id: result.GetProperty("id").GetString()!,
                Url: urls.GetProperty("regular").GetString()!,
                SmallUrl: urls.GetProperty("small").GetString()!,
                AuthorName: user.GetProperty("name").GetString()!,
                AuthorUrl: user.GetProperty("links").GetProperty("html").GetString()!));
        }

        _logger.LogInformation("Searched Unsplash for '{Query}', found {Count} photos", query, photos.Count);
        return photos;
    }

    public async Task<byte[]> DownloadPhotoAsync(string photoId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>(
            $"photos/{photoId}/download",
            cancellationToken);

        var downloadUrl = response.GetProperty("url").GetString()!;

        using var photoResponse = await _httpClient.GetAsync(downloadUrl, cancellationToken);
        photoResponse.EnsureSuccessStatusCode();

        return await photoResponse.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
