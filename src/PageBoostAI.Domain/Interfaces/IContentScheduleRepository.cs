using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Domain.Interfaces;

public interface IContentScheduleRepository : IRepository<ContentSchedule>
{
    Task<IReadOnlyList<ContentSchedule>> GetByPageIdAsync(Guid pageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentSchedule>> GetScheduledPostsAsync(DateTime before, CancellationToken cancellationToken = default);
    Task<int> CountByPageIdAndDateAsync(Guid pageId, DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentSchedule>> GetByStatusAsync(PostStatus status, CancellationToken cancellationToken = default);
}
