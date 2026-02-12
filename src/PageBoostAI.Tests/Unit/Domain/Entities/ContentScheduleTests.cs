using FluentAssertions;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Events;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Domain.Entities;

public class ContentScheduleTests
{
    private static readonly Guid ValidPageId = Guid.NewGuid();
    private static PostContent ValidContent => new("This is a test post for the business page.");

    /// <summary>
    /// Creates a valid scheduled time during business hours (e.g., 10:00 AM tomorrow).
    /// </summary>
    private static DateTime ValidScheduledTime =>
        DateTime.UtcNow.Date.AddDays(1).AddHours(10); // 10 AM tomorrow

    private static ContentSchedule CreateValidSchedule() =>
        new(ValidPageId, ValidContent, ValidScheduledTime);

    [Fact]
    public void Create_WithValidData_ShouldCreateSchedule()
    {
        // Act
        var schedule = CreateValidSchedule();

        // Assert
        schedule.PageId.Should().Be(ValidPageId);
        schedule.Content.Text.Should().Be("This is a test post for the business page.");
        schedule.ScheduledFor.Should().Be(ValidScheduledTime);
        schedule.Status.Should().Be(PostStatus.Scheduled);
        schedule.RetryCount.Should().Be(0);
        schedule.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldSetThem()
    {
        // Arrange
        var hashtags = new List<string> { "#business", "#promo" };

        // Act
        var schedule = new ContentSchedule(
            ValidPageId, ValidContent, ValidScheduledTime,
            imageUrl: "https://example.com/image.jpg",
            hashtags: hashtags,
            callToAction: "Visit our store!");

        // Assert
        schedule.ImageUrl.Should().Be("https://example.com/image.jpg");
        schedule.Hashtags.Should().BeEquivalentTo(hashtags);
        schedule.CallToAction.Should().Be("Visit our store!");
    }

    [Fact]
    public void Create_DuringQuietHours_ShouldThrow()
    {
        // Arrange - 11 PM is during quiet hours (10 PM - 6 AM)
        var quietTime = DateTime.UtcNow.Date.AddDays(1).AddHours(23);

        // Act
        var act = () => new ContentSchedule(ValidPageId, ValidContent, quietTime);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot schedule posts between 10 PM and 6 AM*");
    }

    [Fact]
    public void Create_DuringEarlyMorningQuietHours_ShouldThrow()
    {
        // Arrange - 3 AM is during quiet hours (10 PM - 6 AM)
        var earlyMorning = DateTime.UtcNow.Date.AddDays(1).AddHours(3);

        // Act
        var act = () => new ContentSchedule(ValidPageId, ValidContent, earlyMorning);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Publish_WhenScheduled_ShouldSetStatusPublished()
    {
        // Arrange
        var schedule = CreateValidSchedule();

        // Act
        schedule.Publish("fb_post_123");

        // Assert
        schedule.Status.Should().Be(PostStatus.Published);
        schedule.FacebookPostId.Should().Be("fb_post_123");
        schedule.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void Publish_WhenScheduled_ShouldRaisePostPublishedEvent()
    {
        // Arrange
        var schedule = CreateValidSchedule();

        // Act
        schedule.Publish("fb_post_456");

        // Assert
        schedule.DomainEvents.Should().ContainSingle();
        var domainEvent = schedule.DomainEvents.First();
        domainEvent.Should().BeOfType<PostPublishedEvent>();
        var publishedEvent = (PostPublishedEvent)domainEvent;
        publishedEvent.ContentScheduleId.Should().Be(schedule.Id);
        publishedEvent.PageId.Should().Be(ValidPageId);
        publishedEvent.FacebookPostId.Should().Be("fb_post_456");
    }

    [Fact]
    public void Cancel_WhenScheduled_ShouldSetStatusCancelled()
    {
        // Arrange
        var schedule = CreateValidSchedule();

        // Act
        schedule.Cancel();

        // Assert
        schedule.Status.Should().Be(PostStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenPublished_ShouldThrow()
    {
        // Arrange
        var schedule = CreateValidSchedule();
        schedule.Publish("fb_post_123");

        // Act
        var act = () => schedule.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot cancel a published post*");
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ShouldOverwriteStatus()
    {
        // Arrange
        var schedule = CreateValidSchedule();
        schedule.Publish("fb_post_123");

        // Act - calling Publish again (entity does not guard against double publish)
        schedule.Publish("fb_post_456");

        // Assert
        schedule.FacebookPostId.Should().Be("fb_post_456");
        schedule.Status.Should().Be(PostStatus.Published);
    }

    [Fact]
    public void IncrementRetry_ShouldIncrementCount()
    {
        // Arrange
        var schedule = CreateValidSchedule();

        // Act
        schedule.IncrementRetry();

        // Assert
        schedule.RetryCount.Should().Be(1);
        schedule.Status.Should().Be(PostStatus.Scheduled);
        schedule.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void IncrementRetry_MultipleTimes_ShouldAccumulate()
    {
        // Arrange
        var schedule = CreateValidSchedule();

        // Act
        schedule.IncrementRetry();
        schedule.IncrementRetry();
        schedule.IncrementRetry();

        // Assert
        schedule.RetryCount.Should().Be(3);
    }

    [Fact]
    public void MarkFailed_ShouldSetStatusAndErrorMessage()
    {
        // Arrange
        var schedule = CreateValidSchedule();

        // Act
        schedule.MarkFailed("Facebook API error");

        // Assert
        schedule.Status.Should().Be(PostStatus.Failed);
        schedule.ErrorMessage.Should().Be("Facebook API error");
    }

    [Fact]
    public void CanSchedule_DuringQuietHours_ShouldReturnFalse()
    {
        // Arrange - 10 PM (22:00) is the start of quiet hours
        var quietTime = DateTime.UtcNow.Date.AddDays(1).AddHours(22);

        // Act
        var result = ContentSchedule.CanSchedule(quietTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanSchedule_At5AM_DuringQuietHours_ShouldReturnFalse()
    {
        // Arrange - 5 AM is during quiet hours (before 6 AM)
        var earlyMorning = DateTime.UtcNow.Date.AddDays(1).AddHours(5);

        // Act
        var result = ContentSchedule.CanSchedule(earlyMorning);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanSchedule_DuringBusinessHours_ShouldReturnTrue()
    {
        // Arrange - 2 PM is during business hours
        var businessTime = DateTime.UtcNow.Date.AddDays(1).AddHours(14);

        // Act
        var result = ContentSchedule.CanSchedule(businessTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanSchedule_At6AM_ShouldReturnTrue()
    {
        // Arrange - 6 AM is the boundary (end of quiet hours)
        var sixAm = DateTime.UtcNow.Date.AddDays(1).AddHours(6);

        // Act
        var result = ContentSchedule.CanSchedule(sixAm);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanScheduleMore_WhenUnderLimit_ShouldReturnTrue()
    {
        // Act
        var result = ContentSchedule.CanScheduleMore(3);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanScheduleMore_WhenMaxPostsPerDay_ShouldReturnFalse()
    {
        // Act - 4 is the max posts per day
        var result = ContentSchedule.CanScheduleMore(4);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanScheduleMore_WhenOverLimit_ShouldReturnFalse()
    {
        // Act
        var result = ContentSchedule.CanScheduleMore(5);

        // Assert
        result.Should().BeFalse();
    }
}
