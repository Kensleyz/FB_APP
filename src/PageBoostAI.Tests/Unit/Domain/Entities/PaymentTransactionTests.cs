using FluentAssertions;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Events;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Domain.Entities;

public class PaymentTransactionTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static readonly Guid ValidSubscriptionId = Guid.NewGuid();
    private static Money ValidAmount => Money.ZAR(99.00m);

    private static PaymentTransaction CreateValidTransaction() =>
        new(ValidUserId, ValidSubscriptionId, ValidAmount);

    [Fact]
    public void Create_WithValidData_ShouldCreateTransaction()
    {
        // Act
        var transaction = CreateValidTransaction();

        // Assert
        transaction.UserId.Should().Be(ValidUserId);
        transaction.SubscriptionId.Should().Be(ValidSubscriptionId);
        transaction.Amount.Should().Be(ValidAmount);
        transaction.Currency.Should().Be("ZAR");
        transaction.Status.Should().Be(PaymentStatus.Pending);
        transaction.CompletedAt.Should().BeNull();
        transaction.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldSetThem()
    {
        // Act
        var transaction = new PaymentTransaction(
            ValidUserId, ValidSubscriptionId, ValidAmount,
            payFastPaymentId: "pf_pay_123",
            paymentMethod: "credit_card",
            transactionType: "subscription",
            metadata: "{\"plan\":\"pro\"}");

        // Assert
        transaction.PayFastPaymentId.Should().Be("pf_pay_123");
        transaction.PaymentMethod.Should().Be("credit_card");
        transaction.TransactionType.Should().Be("subscription");
        transaction.Metadata.Should().Be("{\"plan\":\"pro\"}");
    }

    [Fact]
    public void Complete_ShouldSetStatusComplete()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.Complete();

        // Assert
        transaction.Status.Should().Be(PaymentStatus.Complete);
        transaction.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_ShouldRaisePaymentReceivedEvent()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.Complete();

        // Assert
        transaction.DomainEvents.Should().ContainSingle();
        var domainEvent = transaction.DomainEvents.First();
        domainEvent.Should().BeOfType<PaymentReceivedEvent>();
        var paymentEvent = (PaymentReceivedEvent)domainEvent;
        paymentEvent.UserId.Should().Be(ValidUserId);
        paymentEvent.PaymentTransactionId.Should().Be(transaction.Id);
        paymentEvent.Amount.Should().Be(99.00m);
        paymentEvent.Currency.Should().Be("ZAR");
    }

    [Fact]
    public void Fail_ShouldSetStatusFailed()
    {
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.Fail();

        // Assert
        transaction.Status.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public void Refund_WhenComplete_ShouldSetStatusRefunded()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        transaction.Complete();
        transaction.ClearDomainEvents();

        // Act
        transaction.Refund();

        // Assert
        transaction.Status.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void Refund_WhenPending_ShouldSetStatusRefunded()
    {
        // The entity does not guard against refunding a pending transaction,
        // so it will just set the status. This documents the current behavior.
        // Arrange
        var transaction = CreateValidTransaction();

        // Act
        transaction.Refund();

        // Assert
        transaction.Status.Should().Be(PaymentStatus.Refunded);
    }
}
