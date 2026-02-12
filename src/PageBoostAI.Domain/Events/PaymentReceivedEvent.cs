namespace PageBoostAI.Domain.Events;

public sealed class PaymentReceivedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid PaymentTransactionId { get; }
    public decimal Amount { get; }
    public string Currency { get; }

    public PaymentReceivedEvent(Guid userId, Guid paymentTransactionId, decimal amount, string currency)
    {
        UserId = userId;
        PaymentTransactionId = paymentTransactionId;
        Amount = amount;
        Currency = currency;
    }
}
