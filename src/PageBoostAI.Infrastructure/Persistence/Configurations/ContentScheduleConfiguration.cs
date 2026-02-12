using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Infrastructure.Persistence.Configurations;

public class ContentScheduleConfiguration : IEntityTypeConfiguration<ContentSchedule>
{
    public void Configure(EntityTypeBuilder<ContentSchedule> builder)
    {
        builder.ToTable("ContentSchedule");

        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Content)
            .HasConversion(
                content => content.Text,
                text => new PostContent(text))
            .HasMaxLength(PostContent.MaxLength)
            .IsRequired();

        builder.Property(cs => cs.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(cs => cs.Hashtags)
            .HasColumnType("jsonb");

        builder.Property(cs => cs.CallToAction)
            .HasMaxLength(500);

        builder.Property(cs => cs.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(cs => cs.FacebookPostId)
            .HasMaxLength(100);

        builder.Property(cs => cs.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(cs => new { cs.PageId, cs.ScheduledFor, cs.Status })
            .HasDatabaseName("IX_ContentSchedule_PageId_ScheduledFor_Status");

        builder.HasIndex(cs => cs.Status);

        builder.Ignore(cs => cs.DomainEvents);
    }
}
