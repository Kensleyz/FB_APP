using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Domain.Events;

public sealed class SubscriptionChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public SubscriptionTier OldTier { get; }
    public SubscriptionTier NewTier { get; }

    public SubscriptionChangedEvent(Guid userId, SubscriptionTier oldTier, SubscriptionTier newTier)
    {
        UserId = userId;
        OldTier = oldTier;
        NewTier = newTier;
    }
}
