using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
