using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.PaidAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.OrderStatus)
            .IsRequired()
            .HasConversion(
                status => status.ToString(),
                value => (OrderStatus)Enum.Parse(typeof(OrderStatus), value))
            .HasMaxLength(50);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Address)
            .WithMany()
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Quotation)
            .WithOne(q => q.Order)
            .HasForeignKey<Order>(o => o.QuotationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}