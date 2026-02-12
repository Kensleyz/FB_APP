using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Domain.Interfaces;

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PaymentTransaction?> GetByPayFastPaymentIdAsync(string payFastPaymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentTransaction>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
}
