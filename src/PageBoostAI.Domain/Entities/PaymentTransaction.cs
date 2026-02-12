using PageBoostAI.Domain.Common;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Events;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Domain.Entities;

public sealed class PaymentTransaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public string? PayFastPaymentId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string Currency { get; private set; } = "ZAR";
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; private set; }
    public string? TransactionType { get; private set; }
    public string? Metadata { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private PaymentTransaction() { } // EF Core

    public PaymentTransaction(
        Guid userId,
        Guid subscriptionId,
        Money amount,
        string? payFastPaymentId = null,
        string? paymentMethod = null,
        string? transactionType = null,
        string? metadata = null)
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
        Amount = amount;
        Currency = amount.Currency;
        PayFastPaymentId = payFastPaymentId;
        PaymentMethod = paymentMethod;
        TransactionType = transactionType;
        Metadata = metadata;
    }

    public void Complete()
    {
        Status = PaymentStatus.Complete;
        CompletedAt = DateTime.UtcNow;
        SetUpdated();

        AddDomainEvent(new PaymentReceivedEvent(UserId, Id, Amount.Amount, Currency));
    }

    public void Fail()
    {
        Status = PaymentStatus.Failed;
        SetUpdated();
    }

    public void Refund()
    {
        Status = PaymentStatus.Refunded;
        SetUpdated();
    }
}
