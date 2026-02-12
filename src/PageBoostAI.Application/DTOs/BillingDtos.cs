namespace PageBoostAI.Application.DTOs;

// Billing
public record PlanDto(string Tier, string Name, decimal Price, string Currency, int MaxPosts, int MaxPages, int MaxImages, List<string> Features);
public record SubscribeDto(string Tier, string? ReturnUrl = null, string? CancelUrl = null);
public record SubscriptionDto(Guid Id, string Tier, decimal Amount, string Currency, string Status, DateTime StartDate, DateTime? NextBillingDate, DateTime? CancelledAt);
public record UpgradeDto(string NewTier);
public record DowngradeDto(string NewTier);
public record UsageDto(string Period, int PostsGenerated, int PostsLimit, int ImagesCreated, int ImagesLimit, int PostsPublished, int PagesConnected, int PagesLimit);
public record TransactionDto(Guid Id, decimal Amount, string Currency, string Status, string? PaymentMethod, string? TransactionType, DateTime CreatedAt, DateTime? CompletedAt);

// Auth
public record RegisterDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber = null);
public record LoginDto(string Email, string Password);
public record VerifyEmailDto(string Token);
public record RefreshTokenDto(string RefreshToken);
public record ForgotPasswordDto(string Email);
public record ResetPasswordDto(string Token, string NewPassword);
public record AuthResponseDto(string AccessToken, string RefreshToken, UserProfileDto User);
public record UserProfileDto(Guid Id, string Email, string FirstName, string LastName, string? PhoneNumber, string SubscriptionTier, bool IsEmailVerified, DateTime? LastLoginAt, DateTime CreatedAt);

// Facebook
public record ConnectFacebookDto(string RedirectUri);
public record FacebookPageDto(Guid Id, string FacebookPageId, string PageName, string? Category, string? AccessToken, bool IsConnected, DateTime ConnectedAt);
public record PageInsightsDto(Guid PageId, string PageName, int TotalFollowers, int TotalLikes, int PostsPublished, int Reach, int Engagement);

// Content
public record GenerateContentDto(Guid PageId, string BusinessType, string Tone, string PostType, string? AdditionalContext = null);
public record GeneratedContentDto(string Content, string? ImageUrl, List<string>? Hashtags);
public record GenerateImageDto(string Prompt, string? Style = null);
public record GeneratedImageDto(string ImageUrl, string Prompt);
public record ContentTemplateDto(string Id, string Name, string Description, string BusinessType, string Template);

// Schedule
public record CreateScheduleDto(Guid PageId, string Content, DateTime ScheduledFor, string? ImageUrl = null, List<string>? Hashtags = null, string? CallToAction = null);
public record UpdateScheduleDto(string? Content = null, DateTime? ScheduledFor = null, string? ImageUrl = null, List<string>? Hashtags = null, string? CallToAction = null);
public record ScheduleDto(Guid Id, Guid PageId, string PageName, string Content, string Status, DateTime ScheduledFor, DateTime? PublishedAt, string? ImageUrl, List<string>? Hashtags, string? CallToAction, string? FacebookPostId, DateTime CreatedAt);
public record CalendarDto(string Month, List<CalendarDayDto> Days);
public record CalendarDayDto(DateTime Date, List<ScheduleDto> Schedules);

// Dashboard
public record DashboardOverviewDto(int TotalPages, int TotalPosts, int PostsThisMonth, int ScheduledPosts, string SubscriptionTier, UsageDto Usage);
public record AnalyticsDto(Guid PageId, string PageName, string Period, int TotalReach, int TotalEngagement, int PostCount, List<DailyAnalyticsDto> DailyBreakdown);
public record DailyAnalyticsDto(DateTime Date, int Reach, int Engagement, int PostCount);

// Webhooks
public record PayFastNotificationDto(string? m_payment_id, string? pf_payment_id, string? payment_status, string? item_name, string? amount_gross, string? amount_fee, string? amount_net, string? custom_str1, string? signature);
