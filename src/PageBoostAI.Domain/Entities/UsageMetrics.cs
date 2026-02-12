using PageBoostAI.Domain.Common;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Domain.Entities;

public sealed class UsageMetrics : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Period { get; private set; } = string.Empty; // YYYY-MM
    public int PostsGenerated { get; private set; }
    public int ImagesCreated { get; private set; }
    public int PostsPublished { get; private set; }
    public int ApiCallsCount { get; private set; }

    private UsageMetrics() { } // EF Core

    public UsageMetrics(Guid userId, string period)
    {
        UserId = userId;
        Period = period;
    }

    public static UsageMetrics ForCurrentMonth(Guid userId)
    {
        return new UsageMetrics(userId, DateTime.UtcNow.ToString("yyyy-MM"));
    }

    public void IncrementPosts()
    {
        PostsGenerated++;
        ApiCallsCount++;
        SetUpdated();
    }

    public void IncrementImages()
    {
        ImagesCreated++;
        ApiCallsCount++;
        SetUpdated();
    }

    public void IncrementPublished()
    {
        PostsPublished++;
        SetUpdated();
    }

    public bool HasReachedLimit(SubscriptionTier tier)
    {
        var (maxPosts, _, maxImages) = GetLimits(tier);
        return PostsGenerated >= maxPosts || ImagesCreated >= maxImages;
    }

    public static (int MaxPosts, int MaxPages, int MaxImages) GetLimits(SubscriptionTier tier)
    {
        return tier switch
        {
            SubscriptionTier.Free => (5, 1, 2),
            SubscriptionTier.Starter => (10, 1, 5),
            SubscriptionTier.Growth => (30, 3, 15),
            SubscriptionTier.Pro => (100, 10, 50),
            _ => (5, 1, 2)
        };
    }
}
