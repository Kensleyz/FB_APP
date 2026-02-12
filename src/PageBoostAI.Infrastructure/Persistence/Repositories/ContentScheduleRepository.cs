using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Infrastructure.Persistence.Repositories;

public class ContentScheduleRepository : RepositoryBase<ContentSchedule>, IContentScheduleRepository
{
    public ContentScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ContentSchedule>> GetByPageIdAsync(Guid pageId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(cs => cs.PageId == pageId)
            .OrderByDescending(cs => cs.ScheduledFor)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContentSchedule>> GetScheduledPostsAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(cs => cs.Status == PostStatus.Scheduled && cs.ScheduledFor <= before)
            .OrderBy(cs => cs.ScheduledFor)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByPageIdAndDateAsync(Guid pageId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await DbSet.CountAsync(
            cs => cs.PageId == pageId
                  && cs.ScheduledFor >= startOfDay
                  && cs.ScheduledFor < endOfDay
                  && cs.Status != PostStatus.Cancelled,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ContentSchedule>> GetByStatusAsync(PostStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(cs => cs.Status == status)
            .OrderBy(cs => cs.ScheduledFor)
            .ToListAsync(cancellationToken);
    }
}
