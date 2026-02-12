using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PageBoostAI.Infrastructure.ExternalServices;

public interface IAnthropicService
{
    Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GeneratePostVariationsAsync(string businessType, string tone, string topic, int count = 3, CancellationToken cancellationToken = default);
}

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

    public async Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = _model,
            max_tokens = 1024,
            system = SystemPrompt,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("v1/messages", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var content = json.GetProperty("content")[0].GetProperty("text").GetString();

        _logger.LogInformation("Generated content via Anthropic API, model: {Model}", _model);

        return content ?? string.Empty;
    }

    public async Task<IReadOnlyList<string>> GeneratePostVariationsAsync(
        string businessType, string tone, string topic, int count = 3, CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            Generate {count} unique Facebook post variations for a {businessType} in South Africa.

            Topic: {topic}
            Tone: {tone}

            Requirements:
            - Each post should be under 280 characters
            - Include relevant hashtags
            - Make it engaging for South African audiences
            - Each variation should have a different angle or approach

            Return ONLY the posts, separated by "---" on its own line. Do not include numbering or labels.
            """;

        var content = await GenerateContentAsync(prompt, cancellationToken);
        var variations = content
            .Split("---", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        _logger.LogInformation("Generated {Count} post variations for {BusinessType}", variations.Count, businessType);

        return variations;
    }
}
