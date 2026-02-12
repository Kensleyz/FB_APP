using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Infrastructure.Persistence.Repositories;

public class UsageMetricsRepository : RepositoryBase<UsageMetrics>, IUsageMetricsRepository
{
    public UsageMetricsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UsageMetrics?> GetByUserIdAndPeriodAsync(Guid userId, string period, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            um => um.UserId == userId && um.Period == period,
            cancellationToken);
    }

    public async Task<UsageMetrics> GetOrCreateCurrentMonthAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var period = DateTime.UtcNow.ToString("yyyy-MM");
        var metrics = await GetByUserIdAndPeriodAsync(userId, period, cancellationToken);

        if (metrics is not null)
            return metrics;

        metrics = UsageMetrics.ForCurrentMonth(userId);
        await AddAsync(metrics, cancellationToken);
        return metrics;
    }
}
