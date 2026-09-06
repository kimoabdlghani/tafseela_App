using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(o => o.ProductTotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.DeliveryCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.OrderTotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.DepositAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.DepositPercentageSnapshot)
            .HasColumnType("decimal(5,4)");

        // Address stored as JSON snapshot — not a FK anymore
        builder.Property(o => o.AddressSnapshot)
            .IsRequired()
            .HasMaxLength(2000);

        // User relationship
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItems
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Payments
        builder.HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductionJobs
        builder.HasMany(o => o.ProductionJobs)
            .WithOne(pj => pj.Order)
            .HasForeignKey(pj => pj.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}