using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Infrastructure.ExternalServices;

public class AnthropicService : IAnthropicService
{
    private const string SystemPrompt = """
        You are a South African social media content expert specializing in Facebook page management
        for local businesses. You understand the South African market, culture, and local business types
        including spaza shops, hair salons, churches, restaurants, gyms, funeral parlors, taxi associations,
        and general businesses. You write engaging, culturally relevant content that resonates with
        South African audiences. Always consider local slang, trends, and community values where appropriate.
        """;

    private readonly HttpClient _httpClient;
    private readonly ILogger<AnthropicService> _logger;
    private readonly string _model;

    public AnthropicService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AnthropicService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _model = configuration["ANTHROPIC_MODEL"] ?? "claude-sonnet-4-5-20250929";

        _httpClient.BaseAddress = new Uri("https://api.anthropic.com/");
        _httpClient.DefaultRequestHeaders.Add("x-api-key", configuration["ANTHROPIC_API_KEY"]);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
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
        var prompt = $$"""
            Generate 3 unique Facebook post variations for a {{businessType}} business in South Africa.

            Business Name: {{businessName}}
            Business Description: {{businessDescription}}
            Post Type: {{postType}}
            Tone: {{tone}}
            Language: {{language}}

            For each variation return exactly this JSON format (return a JSON array, nothing else):
            [
              {
                "content": "post text here",
                "hashtags": ["hashtag1", "hashtag2"],
                "callToAction": "CTA text here"
              }
            ]
            """;

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
            _logger.LogError(ex, "Failed to parse Anthropic response as JSON, falling back to text parsing");
            return [new PostVariation(content, [], string.Empty)];
        }
    }

    private async Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _model,
            max_tokens = 1024,
            system = SystemPrompt,
            messages = new[] { new { role = "user", content = prompt } }
        };

        var response = await _httpClient.PostAsJsonAsync("v1/messages", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var text = json.GetProperty("content")[0].GetProperty("text").GetString();

        _logger.LogInformation("Generated content via Anthropic API, model: {Model}", _model);
        return text ?? string.Empty;
    }

    private sealed class PostVariationJson
    {
        public string? Content { get; init; }
        public List<string>? Hashtags { get; init; }
        public string? CallToAction { get; init; }
    }
}
