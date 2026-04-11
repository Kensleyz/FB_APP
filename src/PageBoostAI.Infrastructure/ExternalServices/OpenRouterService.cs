using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Infrastructure.ExternalServices;

// OpenRouter — routes to many providers, has free models (e.g. mistralai/mistral-7b-instruct:free)
public class OpenRouterService : IAIService
{
    private const string SystemPrompt =
        "You are a South African social media content expert specializing in Facebook page management " +
        "for local businesses. You understand the South African market, culture, and local business types " +
        "including spaza shops, hair salons, churches, restaurants, gyms, funeral parlors, taxi associations, " +
        "and general businesses. You write engaging, culturally relevant content that resonates with " +
        "South African audiences. Always consider local slang, trends, and community values where appropriate.";

    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenRouterService> _logger;
    private readonly string _model;

    public OpenRouterService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenRouterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _model = configuration["OPENROUTER_MODEL"] ?? "mistralai/mistral-7b-instruct:free";

        var baseUrl = configuration["OPENROUTER_BASE_URL"] ?? configuration["AISettings:OpenRouterBaseUrl"]
            ?? throw new InvalidOperationException("OpenRouter base URL is not configured.");
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["OPENROUTER_API_KEY"]}");
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", configuration["FRONTEND_URL"] ?? "https://fb-app-azure.vercel.app");
    }

    public async Task<List<PostVariation>> GeneratePostsAsync(
        BusinessType businessType,
        ToneOption tone,
        PostType postType,
        string language,
        string businessName,
        string businessDescription,
        CancellationToken cancellationToken = default)
    {
        var prompt =
            $"Generate 3 unique Facebook post variations for a {businessType} business in South Africa.\n\n" +
            $"Business Name: {businessName}\n" +
            $"Business Description: {businessDescription}\n" +
            $"Post Type: {postType}\n" +
            $"Tone: {tone}\n" +
            $"Language: {language}\n\n" +
            "For each variation return exactly this JSON format (return a JSON array, nothing else):\n" +
            "[\n  {\n    \"content\": \"post text here\",\n    \"hashtags\": [\"hashtag1\", \"hashtag2\"],\n    \"callToAction\": \"CTA text here\"\n  }\n]";

        var content = await GenerateContentAsync(prompt, cancellationToken);

        try
        {
            var start = content.IndexOf('[');
            var end = content.LastIndexOf(']') + 1;
            var json = content[start..end];

            var variations = JsonSerializer.Deserialize<List<PostVariationJson>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

            return variations.Select(v => new PostVariation(
                v.Content ?? string.Empty,
                v.Hashtags ?? [],
                v.CallToAction ?? string.Empty)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse OpenRouter response as JSON, falling back to text");
            return [new PostVariation(content, [], string.Empty)];
        }
    }

    private async Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _model,
            max_tokens = 1024,
            messages = new[]
            {
                new { role = "system", content = SystemPrompt },
                new { role = "user", content = prompt }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("OpenRouter returned {StatusCode}: {Body}", (int)response.StatusCode, errorBody);
            throw new InvalidOperationException($"OpenRouter error ({(int)response.StatusCode}): {errorBody}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var text = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        _logger.LogInformation("Generated content via OpenRouter, model: {Model}", _model);
        return text ?? string.Empty;
    }

    private sealed record PostVariationJson(string? Content, List<string>? Hashtags, string? CallToAction);
}
