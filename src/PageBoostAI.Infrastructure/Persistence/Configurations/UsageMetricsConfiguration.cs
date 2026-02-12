using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Infrastructure.Persistence.Configurations;

public class UsageMetricsConfiguration : IEntityTypeConfiguration<UsageMetrics>
{
    public void Configure(EntityTypeBuilder<UsageMetrics> builder)
    {
        builder.ToTable("UsageMetrics");

        builder.HasKey(um => um.Id);

        builder.Property(um => um.Period)
            .HasMaxLength(7) // YYYY-MM
            .IsRequired();

        builder.HasIndex(um => new { um.UserId, um.Period }).IsUnique();

        builder.Ignore(um => um.DomainEvents);
    }
}
