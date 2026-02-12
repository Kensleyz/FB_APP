using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Domain.Interfaces;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Subscription?> GetByPayFastTokenAsync(string token, CancellationToken cancellationToken = default);
}
