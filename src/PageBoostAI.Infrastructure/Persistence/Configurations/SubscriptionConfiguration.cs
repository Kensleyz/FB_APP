using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PayFastSubscriptionToken)
            .HasMaxLength(255);

        builder.HasIndex(s => s.PayFastSubscriptionToken)
            .HasFilter("\"PayFastSubscriptionToken\" IS NOT NULL");

        builder.Property(s => s.Tier)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Amount)
            .HasConversion(
                money => money.Amount,
                value => new Money(value, "ZAR"))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Currency)
            .HasMaxLength(3)
            .HasDefaultValue("ZAR")
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(s => new { s.UserId, s.Status })
            .HasDatabaseName("IX_Subscriptions_UserId_Status");

        builder.Ignore(s => s.DomainEvents);
    }
}
