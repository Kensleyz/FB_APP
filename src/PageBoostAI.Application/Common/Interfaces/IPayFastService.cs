using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Application.Common.Interfaces;

public record PayFastSubscriptionResult(string SubscriptionToken, string RedirectUrl);

public interface IPayFastService
{
    Task<PayFastSubscriptionResult> CreateSubscriptionAsync(Guid userId, SubscriptionTier tier, string email, CancellationToken cancellationToken = default);
    Task CancelSubscriptionAsync(string subscriptionToken, CancellationToken cancellationToken = default);
    bool ValidateWebhook(Dictionary<string, string> formData, string signature);
    string GenerateSignature(Dictionary<string, string> data);
}
