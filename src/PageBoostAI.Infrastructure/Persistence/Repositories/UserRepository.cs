using Microsoft.EntityFrameworkCore;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Infrastructure.Persistence.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.EmailVerificationToken == token, cancellationToken);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}
