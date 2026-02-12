using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Domain.Interfaces;

public interface IFacebookPageRepository : IRepository<FacebookPage>
{
    Task<IReadOnlyList<FacebookPage>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FacebookPage?> GetByFacebookPageIdAsync(string facebookPageId, CancellationToken cancellationToken = default);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
