namespace PageBoostAI.Application.Billing.DTOs;

public record PlanDto(string Tier, string Name, decimal Price, string Currency, int MaxPosts, int MaxPages, int MaxImages, List<string> Features);
public record SubscribeRequestDto(string Tier, string? ReturnUrl = null, string? CancelUrl = null);
public record SubscriptionDto(Guid Id, string Tier, decimal Amount, string Currency, string Status, DateTime StartDate, DateTime? NextBillingDate, DateTime? CancelledAt);
public record UpgradeRequestDto(string NewTier);
public record DowngradeRequestDto(string NewTier);
public record UsageDto(string Period, int PostsGenerated, int PostsLimit, int ImagesCreated, int ImagesLimit, int PostsPublished, int PagesConnected, int PagesLimit);
public record TransactionDto(Guid Id, decimal Amount, string Currency, string Status, string? PaymentMethod, string? TransactionType, DateTime CreatedAt, DateTime? CompletedAt);
