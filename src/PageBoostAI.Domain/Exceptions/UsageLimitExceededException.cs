using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Domain.Exceptions;

public sealed class UsageLimitExceededException : DomainException
{
    public SubscriptionTier Tier { get; }
    public string LimitType { get; }

    public UsageLimitExceededException(SubscriptionTier tier, string limitType)
        : base($"Usage limit for '{limitType}' has been reached on the {tier} plan.")
    {
        Tier = tier;
        LimitType = limitType;
    }
}
