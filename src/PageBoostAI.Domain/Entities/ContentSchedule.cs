using PageBoostAI.Domain.Common;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Events;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Domain.Entities;

public sealed class ContentSchedule : BaseEntity
{
    private const int MaxPostsPerDay = 4;
    private static readonly TimeOnly QuietStart = new(22, 0); // 10 PM
    private static readonly TimeOnly QuietEnd = new(6, 0);    // 6 AM

    public Guid PageId { get; private set; }
    public PostContent Content { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public List<string> Hashtags { get; private set; } = new();
    public string? CallToAction { get; private set; }
    public DateTime ScheduledFor { get; private set; }
    public PostStatus Status { get; private set; } = PostStatus.Scheduled;
    public string? FacebookPostId { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    private ContentSchedule() { } // EF Core

    public ContentSchedule(
        Guid pageId,
        PostContent content,
        DateTime scheduledFor,
        string? imageUrl = null,
        List<string>? hashtags = null,
        string? callToAction = null)
    {
        if (!CanSchedule(scheduledFor))
            throw new DomainException("Cannot schedule posts between 10 PM and 6 AM.");

        PageId = pageId;
        Content = content;
        ScheduledFor = scheduledFor;
        ImageUrl = imageUrl;
        Hashtags = hashtags ?? new List<string>();
        CallToAction = callToAction;
    }

    public void Update(
        PostContent? content = null,
        DateTime? scheduledFor = null,
        string? imageUrl = null,
        List<string>? hashtags = null,
        string? callToAction = null)
    {
        if (Status != PostStatus.Scheduled)
            throw new DomainException("Can only update posts with Scheduled status.");

        if (scheduledFor.HasValue && !CanSchedule(scheduledFor.Value))
            throw new DomainException("Cannot schedule posts between 10 PM and 6 AM.");

        if (content is not null) Content = content;
        if (scheduledFor.HasValue) ScheduledFor = scheduledFor.Value;
        if (imageUrl is not null) ImageUrl = imageUrl;
        if (hashtags is not null) Hashtags = hashtags;
        if (callToAction is not null) CallToAction = callToAction;
        SetUpdated();
    }

    public void Publish(string facebookPostId)
    {
        Status = PostStatus.Published;
        FacebookPostId = facebookPostId;
        PublishedAt = DateTime.UtcNow;
        SetUpdated();

        AddDomainEvent(new PostPublishedEvent(Id, PageId, facebookPostId));
    }

    public void Cancel()
    {
        if (Status == PostStatus.Published)
            throw new DomainException("Cannot cancel a published post.");

        Status = PostStatus.Cancelled;
        SetUpdated();
    }

    public void MarkFailed(string errorMessage)
    {
        Status = PostStatus.Failed;
        ErrorMessage = errorMessage;
        SetUpdated();
    }

    public void IncrementRetry()
    {
        RetryCount++;
        Status = PostStatus.Scheduled;
        ErrorMessage = null;
        SetUpdated();
    }

    public static bool CanSchedule(DateTime scheduledFor)
    {
        var time = TimeOnly.FromDateTime(scheduledFor);
        // Quiet hours: 10 PM to 6 AM (cannot schedule during this window)
        if (time >= QuietStart || time < QuietEnd)
            return false;

        return true;
    }

    public static bool CanScheduleMore(int postsScheduledToday)
    {
        return postsScheduledToday < MaxPostsPerDay;
    }
}
