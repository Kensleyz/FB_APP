using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Infrastructure.Persistence.Configurations;

public class FacebookPageConfiguration : IEntityTypeConfiguration<FacebookPage>
{
    public void Configure(EntityTypeBuilder<FacebookPage> builder)
    {
        builder.ToTable("FacebookPages");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.FacebookPageId)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(fp => fp.FacebookPageId).IsUnique();

        builder.Property(fp => fp.PageName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(fp => fp.PageCategory)
            .HasMaxLength(100);

        // Access token stored encrypted at application level
        builder.Property(fp => fp.PageAccessToken)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(fp => fp.ProfilePictureUrl)
            .HasMaxLength(2048);

        builder.HasIndex(fp => new { fp.UserId, fp.IsActive })
            .HasDatabaseName("IX_FacebookPages_UserId_IsActive");

        builder.Ignore(fp => fp.DomainEvents);
    }
}
