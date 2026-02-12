using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Infrastructure.Persistence.Repositories;

public class PaymentTransactionRepository : RepositoryBase<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(pt => pt.UserId == userId)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentTransaction?> GetByPayFastPaymentIdAsync(string payFastPaymentId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            pt => pt.PayFastPaymentId == payFastPaymentId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(pt => pt.SubscriptionId == subscriptionId)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
