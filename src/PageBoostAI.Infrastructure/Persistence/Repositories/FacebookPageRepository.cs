using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Infrastructure.Persistence.Repositories;

public class FacebookPageRepository : RepositoryBase<FacebookPage>, IFacebookPageRepository
{
    public FacebookPageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<FacebookPage>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(fp => fp.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<FacebookPage?> GetByFacebookPageIdAsync(string facebookPageId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(fp => fp.FacebookPageId == facebookPageId, cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(fp => fp.UserId == userId && fp.IsActive, cancellationToken);
    }
}
