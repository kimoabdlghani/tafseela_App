using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(200);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        // Unique constraint on GatewayTransactionId for idempotency
        builder.HasIndex(p => p.GatewayTransactionId)
            .IsUnique()
            .HasFilter("[GatewayTransactionId] IS NOT NULL");
    }
}