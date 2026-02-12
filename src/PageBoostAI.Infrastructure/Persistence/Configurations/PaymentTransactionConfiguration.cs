using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.PayFastPaymentId)
            .HasMaxLength(255);

        builder.HasIndex(pt => pt.PayFastPaymentId)
            .HasFilter("\"PayFastPaymentId\" IS NOT NULL");

        builder.Property(pt => pt.Amount)
            .HasConversion(
                money => money.Amount,
                value => new Money(value, "ZAR"))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(pt => pt.Currency)
            .HasMaxLength(3)
            .HasDefaultValue("ZAR")
            .IsRequired();

        builder.Property(pt => pt.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(pt => pt.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(pt => pt.TransactionType)
            .HasMaxLength(50);

        builder.Property(pt => pt.Metadata)
            .HasColumnType("jsonb");

        builder.HasIndex(pt => pt.UserId);
        builder.HasIndex(pt => pt.SubscriptionId);
        builder.HasIndex(pt => pt.Status);

        builder.Ignore(pt => pt.DomainEvents);
    }
}
