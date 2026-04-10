using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Infrastructure.ExternalServices;

public class PayFastService : IPayFastService
{
    private readonly ILogger<PayFastService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _merchantId;
    private readonly string _merchantKey;
    private readonly string _passphrase;
    private readonly string _baseUrl;
    private readonly string _notifyUrl;

    public PayFastService(IConfiguration configuration, ILogger<PayFastService> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _merchantId = configuration["PAYFAST_MERCHANT_ID"] ?? string.Empty;
        _merchantKey = configuration["PAYFAST_MERCHANT_KEY"] ?? string.Empty;
        _passphrase = configuration["PAYFAST_PASSPHRASE"] ?? string.Empty;
        _notifyUrl = configuration["PAYFAST_NOTIFY_URL"] ?? string.Empty;

        var mode = configuration["PAYFAST_MODE"] ?? "sandbox";
        _baseUrl = mode == "live"
            ? "https://www.payfast.co.za/eng/process"
            : "https://sandbox.payfast.co.za/eng/process";
    }

    public Task<PayFastSubscriptionResult> CreateSubscriptionAsync(
        Guid userId, SubscriptionTier tier, string email, CancellationToken cancellationToken = default)
    {
        var amount = GetTierAmount(tier);
        var data = new SortedDictionary<string, string>
        {
            ["merchant_id"] = _merchantId,
            ["merchant_key"] = _merchantKey,
            ["notify_url"] = _notifyUrl,
            ["m_payment_id"] = userId.ToString(),
            ["amount"] = amount.ToString("F2"),
            ["item_name"] = $"PageBoost AI - {tier} Plan",
            ["subscription_type"] = "1",
            ["billing_date"] = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            ["recurring_amount"] = amount.ToString("F2"),
            ["frequency"] = "3",
            ["cycles"] = "0",
            ["email_address"] = email
        };

        var signature = GenerateSignatureInternal(data);
        data["signature"] = signature;

        var queryString = string.Join("&", data.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var redirectUrl = $"{_baseUrl}?{queryString}";

        _logger.LogInformation("Created PayFast subscription URL for user {UserId}, tier {Tier}", userId, tier);

        return Task.FromResult(new PayFastSubscriptionResult(string.Empty, redirectUrl));
    }

    public async Task CancelSubscriptionAsync(string subscriptionToken, CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var data = new SortedDictionary<string, string>
        {
            ["merchant-id"] = _merchantId,
            ["timestamp"] = timestamp,
            ["version"] = "v1"
        };

        var signature = GenerateSignatureInternal(data);

        var request = new HttpRequestMessage(HttpMethod.Put,
            $"https://api.payfast.co.za/subscriptions/{subscriptionToken}/cancel");
        request.Headers.Add("merchant-id", _merchantId);
        request.Headers.Add("timestamp", timestamp);
        request.Headers.Add("version", "v1");
        request.Headers.Add("signature", signature);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        _logger.LogInformation("Cancelled PayFast subscription {Token}, status {Status}",
            subscriptionToken, response.StatusCode);
    }

    public bool ValidateWebhook(Dictionary<string, string> formData, string signature)
    {
        var sorted = new SortedDictionary<string, string>(
            formData.Where(kvp => kvp.Key != "signature")
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

        var expected = GenerateSignatureInternal(sorted);
        return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateSignature(Dictionary<string, string> data)
    {
        var sorted = new SortedDictionary<string, string>(data);
        return GenerateSignatureInternal(sorted);
    }

    private string GenerateSignatureInternal(SortedDictionary<string, string> data)
    {
        var paramString = string.Join("&", data.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        if (!string.IsNullOrEmpty(_passphrase))
            paramString += $"&passphrase={Uri.EscapeDataString(_passphrase)}";

        var hash = MD5.HashData(Encoding.UTF8.GetBytes(paramString));
        return Convert.ToHexStringLower(hash);
    }

    private static decimal GetTierAmount(SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Starter => 199m,
        SubscriptionTier.Growth => 499m,
        SubscriptionTier.Pro => 999m,
        _ => 0m
    };
}
