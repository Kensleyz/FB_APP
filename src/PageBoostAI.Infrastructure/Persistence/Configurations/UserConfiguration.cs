using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.SubscriptionTier)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(u => new { u.SubscriptionTier, u.SubscriptionExpiresAt })
            .HasDatabaseName("IX_Users_SubscriptionTier_ExpiresAt");

        builder.Property(u => u.EmailVerificationToken)
            .HasMaxLength(100);

        builder.HasIndex(u => u.EmailVerificationToken)
            .HasFilter("\"EmailVerificationToken\" IS NOT NULL");

        builder.Property(u => u.PasswordResetToken)
            .HasMaxLength(100);

        builder.HasIndex(u => u.PasswordResetToken)
            .HasFilter("\"PasswordResetToken\" IS NOT NULL");

        builder.Ignore(u => u.DomainEvents);
    }
}
