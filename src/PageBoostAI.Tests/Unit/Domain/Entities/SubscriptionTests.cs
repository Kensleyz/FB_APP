using FluentAssertions;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Domain.Entities;

public class SubscriptionTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static Money ValidAmount => Money.ZAR(99.00m);
    private static readonly DateTime NextBilling = DateTime.UtcNow.AddMonths(1);

    private static Subscription CreateValidSubscription(
        SubscriptionTier tier = SubscriptionTier.Starter) =>
        new(ValidUserId, tier, ValidAmount, nextBillingDate: NextBilling);

    [Fact]
    public void Create_WithValidData_ShouldCreateSubscription()
    {
        // Act
        var subscription = CreateValidSubscription();

        // Assert
        subscription.UserId.Should().Be(ValidUserId);
        subscription.Tier.Should().Be(SubscriptionTier.Starter);
        subscription.Amount.Should().Be(ValidAmount);
        subscription.Currency.Should().Be("ZAR");
        subscription.Status.Should().Be(SubscriptionStatus.Active);
        subscription.NextBillingDate.Should().Be(NextBilling);
        subscription.CancelledAt.Should().BeNull();
        subscription.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithPayFastToken_ShouldSetToken()
    {
        // Act
        var subscription = new Subscription(
            ValidUserId, SubscriptionTier.Growth, ValidAmount,
            payFastSubscriptionToken: "pf_token_123");

        // Assert
        subscription.PayFastSubscriptionToken.Should().Be("pf_token_123");
    }

    [Fact]
    public void Cancel_WhenActive_ShouldSetStatusCancelled()
    {
        // Arrange
        var subscription = CreateValidSubscription();

        // Act
        subscription.Cancel();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Cancelled);
        subscription.CancelledAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrow()
    {
        // Arrange
        var subscription = CreateValidSubscription();
        subscription.Cancel();

        // Act
        var act = () => subscription.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*already cancelled*");
    }

    [Fact]
    public void Upgrade_ToHigherTier_ShouldUpdateTier()
    {
        // Arrange
        var subscription = CreateValidSubscription(SubscriptionTier.Starter);
        var newAmount = Money.ZAR(199.00m);
        var nextBilling = DateTime.UtcNow.AddMonths(1);

        // Act
        subscription.Upgrade(SubscriptionTier.Growth, newAmount, nextBilling);

        // Assert
        subscription.Tier.Should().Be(SubscriptionTier.Growth);
        subscription.Amount.Should().Be(newAmount);
        subscription.NextBillingDate.Should().Be(nextBilling);
        subscription.Status.Should().Be(SubscriptionStatus.Active);
    }

    [Fact]
    public void Upgrade_ToLowerTier_ShouldThrow()
    {
        // Arrange
        var subscription = CreateValidSubscription(SubscriptionTier.Growth);
        var newAmount = Money.ZAR(99.00m);

        // Act
        var act = () => subscription.Upgrade(SubscriptionTier.Starter, newAmount, DateTime.UtcNow.AddMonths(1));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot upgrade*");
    }

    [Fact]
    public void Upgrade_ToSameTier_ShouldThrow()
    {
        // Arrange
        var subscription = CreateValidSubscription(SubscriptionTier.Starter);
        var newAmount = Money.ZAR(99.00m);

        // Act
        var act = () => subscription.Upgrade(SubscriptionTier.Starter, newAmount, DateTime.UtcNow.AddMonths(1));

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Downgrade_ToLowerTier_ShouldUpdateTier()
    {
        // Arrange
        var subscription = CreateValidSubscription(SubscriptionTier.Pro);
        var newAmount = Money.ZAR(199.00m);
        var nextBilling = DateTime.UtcNow.AddMonths(1);

        // Act
        subscription.Downgrade(SubscriptionTier.Growth, newAmount, nextBilling);

        // Assert
        subscription.Tier.Should().Be(SubscriptionTier.Growth);
        subscription.Amount.Should().Be(newAmount);
        subscription.NextBillingDate.Should().Be(nextBilling);
    }

    [Fact]
    public void Downgrade_ToHigherTier_ShouldThrow()
    {
        // Arrange
        var subscription = CreateValidSubscription(SubscriptionTier.Starter);
        var newAmount = Money.ZAR(299.00m);

        // Act
        var act = () => subscription.Downgrade(SubscriptionTier.Growth, newAmount, DateTime.UtcNow.AddMonths(1));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot downgrade*");
    }

    [Fact]
    public void Downgrade_ToSameTier_ShouldThrow()
    {
        // Arrange
        var subscription = CreateValidSubscription(SubscriptionTier.Starter);

        // Act
        var act = () => subscription.Downgrade(SubscriptionTier.Starter, ValidAmount, DateTime.UtcNow.AddMonths(1));

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void IsActive_WhenActive_ShouldReturnTrue()
    {
        // Arrange
        var subscription = CreateValidSubscription();

        // Act
        var result = subscription.IsActive();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenCancelled_ShouldReturnFalse()
    {
        // Arrange
        var subscription = CreateValidSubscription();
        subscription.Cancel();

        // Act
        var result = subscription.IsActive();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenSuspended_ShouldReturnFalse()
    {
        // Arrange
        var subscription = CreateValidSubscription();
        subscription.Suspend();

        // Act
        var result = subscription.IsActive();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Suspend_ShouldSetStatusSuspended()
    {
        // Arrange
        var subscription = CreateValidSubscription();

        // Act
        subscription.Suspend();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Suspended);
    }

    [Fact]
    public void MarkPastDue_ShouldSetStatusPastDue()
    {
        // Arrange
        var subscription = CreateValidSubscription();

        // Act
        subscription.MarkPastDue();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.PastDue);
    }
}
