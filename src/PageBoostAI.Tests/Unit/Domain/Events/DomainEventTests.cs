using FluentAssertions;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Events;

namespace PageBoostAI.Tests.Unit.Domain.Events;

public class DomainEventTests
{
    [Fact]
    public void UserRegisteredEvent_ShouldContainUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";

        // Act
        var evt = new UserRegisteredEvent(userId, email);

        // Assert
        evt.UserId.Should().Be(userId);
        evt.Email.Should().Be(email);
        evt.EventId.Should().NotBeEmpty();
        evt.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void SubscriptionChangedEvent_ShouldContainOldAndNewTier()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var evt = new SubscriptionChangedEvent(userId, SubscriptionTier.Free, SubscriptionTier.Pro);

        // Assert
        evt.UserId.Should().Be(userId);
        evt.OldTier.Should().Be(SubscriptionTier.Free);
        evt.NewTier.Should().Be(SubscriptionTier.Pro);
        evt.EventId.Should().NotBeEmpty();
        evt.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void PostPublishedEvent_ShouldContainPostId()
    {
        // Arrange
        var contentScheduleId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var facebookPostId = "fb_post_789";

        // Act
        var evt = new PostPublishedEvent(contentScheduleId, pageId, facebookPostId);

        // Assert
        evt.ContentScheduleId.Should().Be(contentScheduleId);
        evt.PageId.Should().Be(pageId);
        evt.FacebookPostId.Should().Be(facebookPostId);
        evt.EventId.Should().NotBeEmpty();
        evt.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void PaymentReceivedEvent_ShouldContainTransactionId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        // Act
        var evt = new PaymentReceivedEvent(userId, transactionId, 199.99m, "ZAR");

        // Assert
        evt.UserId.Should().Be(userId);
        evt.PaymentTransactionId.Should().Be(transactionId);
        evt.Amount.Should().Be(199.99m);
        evt.Currency.Should().Be("ZAR");
        evt.EventId.Should().NotBeEmpty();
        evt.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void DomainEvent_EventId_ShouldBeUnique()
    {
        // Arrange & Act
        var evt1 = new UserRegisteredEvent(Guid.NewGuid(), "a@b.com");
        var evt2 = new UserRegisteredEvent(Guid.NewGuid(), "c@d.com");

        // Assert
        evt1.EventId.Should().NotBe(evt2.EventId);
    }

    [Fact]
    public void DomainEvent_OccurredAt_ShouldBeUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var evt = new UserRegisteredEvent(Guid.NewGuid(), "test@test.com");

        // Assert
        evt.OccurredAt.Should().BeOnOrAfter(before);
        evt.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
