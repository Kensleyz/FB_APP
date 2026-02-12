using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Domain.Interfaces;

public interface IUsageMetricsRepository : IRepository<UsageMetrics>
{
    Task<UsageMetrics?> GetByUserIdAndPeriodAsync(Guid userId, string period, CancellationToken cancellationToken = default);
    Task<UsageMetrics> GetOrCreateCurrentMonthAsync(Guid userId, CancellationToken cancellationToken = default);
}
