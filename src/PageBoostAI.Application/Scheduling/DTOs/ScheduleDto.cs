namespace PageBoostAI.Application.Scheduling.DTOs;

public record ScheduleDto(
    Guid Id, Guid PageId, string Content, string? ImageUrl, List<string> Hashtags,
    string? CallToAction, DateTime ScheduledFor, string Status, string? FacebookPostId,
    string? ErrorMessage, int RetryCount, DateTime? PublishedAt, DateTime CreatedAt);

public record CreateScheduleRequestDto(
    Guid PageId, string Content, DateTime ScheduledFor,
    string? ImageUrl = null, List<string>? Hashtags = null, string? CallToAction = null);

public record UpdateScheduleRequestDto(
    string? Content = null, DateTime? ScheduledFor = null,
    string? ImageUrl = null, List<string>? Hashtags = null, string? CallToAction = null);

public record CalendarDto(string Month, List<ScheduleDto> ScheduledPosts);
