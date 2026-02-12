using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PageBoostAI.Infrastructure.ExternalServices;

public interface IPayFastService
{
    string GeneratePaymentUrl(string merchantReference, decimal amount, string itemName, string returnUrl, string cancelUrl, string notifyUrl);
    bool ValidateSignature(IDictionary<string, string> data, string signature);
}

public class PayFastService : IPayFastService
{
    private readonly ILogger<PayFastService> _logger;
    private readonly string _merchantId;
    private readonly string _merchantKey;
    private readonly string _passphrase;
    private readonly string _baseUrl;

    public PayFastService(IConfiguration configuration, ILogger<PayFastService> logger)
    {
        _logger = logger;
        _merchantId = configuration["PAYFAST_MERCHANT_ID"] ?? string.Empty;
        _merchantKey = configuration["PAYFAST_MERCHANT_KEY"] ?? string.Empty;
        _passphrase = configuration["PAYFAST_PASSPHRASE"] ?? string.Empty;

        var mode = configuration["PAYFAST_MODE"] ?? "sandbox";
        _baseUrl = mode == "live"
            ? "https://www.payfast.co.za/eng/process"
            : "https://sandbox.payfast.co.za/eng/process";
    }

    public string GeneratePaymentUrl(string merchantReference, decimal amount, string itemName, string returnUrl, string cancelUrl, string notifyUrl)
    {
        var data = new SortedDictionary<string, string>
        {
            ["merchant_id"] = _merchantId,
            ["merchant_key"] = _merchantKey,
            ["return_url"] = returnUrl,
            ["cancel_url"] = cancelUrl,
            ["notify_url"] = notifyUrl,
            ["m_payment_id"] = merchantReference,
            ["amount"] = amount.ToString("F2"),
            ["item_name"] = itemName
        };

        var signature = GenerateSignature(data);
        data["signature"] = signature;

        var queryString = string.Join("&", data.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var url = $"{_baseUrl}?{queryString}";

        _logger.LogInformation("Generated PayFast payment URL for reference {Reference}, amount {Amount}", merchantReference, amount);

        return url;
    }

    public bool ValidateSignature(IDictionary<string, string> data, string signature)
    {
        var sorted = new SortedDictionary<string, string>(data.Where(kvp => kvp.Key != "signature").ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        var expectedSignature = GenerateSignature(sorted);
        return string.Equals(expectedSignature, signature, StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateSignature(SortedDictionary<string, string> data)
    {
        var paramString = string.Join("&", data.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        if (!string.IsNullOrEmpty(_passphrase))
            paramString += $"&passphrase={Uri.EscapeDataString(_passphrase)}";

        var hash = MD5.HashData(Encoding.UTF8.GetBytes(paramString));
        return Convert.ToHexStringLower(hash);
    }
}
