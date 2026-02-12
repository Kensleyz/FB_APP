using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Infrastructure.Persistence.Repositories;

public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            s => s.UserId == userId && s.Status == SubscriptionStatus.Active,
            cancellationToken);
    }

    public async Task<Subscription?> GetByPayFastTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            s => s.PayFastSubscriptionToken == token,
            cancellationToken);
    }
}
