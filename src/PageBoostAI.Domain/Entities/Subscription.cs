using PageBoostAI.Domain.Common;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Domain.Entities;

public sealed class Subscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public string? PayFastSubscriptionToken { get; private set; }
    public SubscriptionTier Tier { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string Currency { get; private set; } = "ZAR";
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public DateTime StartDate { get; private set; } = DateTime.UtcNow;
    public DateTime? NextBillingDate { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private Subscription() { } // EF Core

    public Subscription(
        Guid userId,
        SubscriptionTier tier,
        Money amount,
        string? payFastSubscriptionToken = null,
        DateTime? nextBillingDate = null)
    {
        UserId = userId;
        Tier = tier;
        Amount = amount;
        Currency = amount.Currency;
        PayFastSubscriptionToken = payFastSubscriptionToken;
        NextBillingDate = nextBillingDate;
    }

    public void Cancel()
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new DomainException("Subscription is already cancelled.");

        Status = SubscriptionStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        SetUpdated();
    }

    public void Upgrade(SubscriptionTier newTier, Money newAmount, DateTime nextBilling)
    {
        if (newTier <= Tier)
            throw new DomainException($"Cannot upgrade from {Tier} to {newTier}.");

        Tier = newTier;
        Amount = newAmount;
        Currency = newAmount.Currency;
        NextBillingDate = nextBilling;
        Status = SubscriptionStatus.Active;
        SetUpdated();
    }

    public void Downgrade(SubscriptionTier newTier, Money newAmount, DateTime nextBilling)
    {
        if (newTier >= Tier)
            throw new DomainException($"Cannot downgrade from {Tier} to {newTier}.");

        Tier = newTier;
        Amount = newAmount;
        Currency = newAmount.Currency;
        NextBillingDate = nextBilling;
        SetUpdated();
    }

    public bool IsActive()
    {
        return Status == SubscriptionStatus.Active;
    }

    public void Suspend()
    {
        Status = SubscriptionStatus.Suspended;
        SetUpdated();
    }

    public void MarkPastDue()
    {
        Status = SubscriptionStatus.PastDue;
        SetUpdated();
    }
}
