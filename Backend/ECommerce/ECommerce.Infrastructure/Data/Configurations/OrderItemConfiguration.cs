using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        // Snapshot text fields
        builder.Property(oi => oi.ProductNameSnapshot).IsRequired().HasMaxLength(200);
        builder.Property(oi => oi.ProductDescriptionSnapshot).HasMaxLength(2000);
        builder.Property(oi => oi.WoodMaterialNameSnapshot).IsRequired().HasMaxLength(100);
        builder.Property(oi => oi.WoodColorNameSnapshot).IsRequired().HasMaxLength(100);

        // JSON snapshot columns
        builder.Property(oi => oi.DimensionsSnapshot)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(oi => oi.ComponentsSnapshot)
            .HasMaxLength(4000);

        // Decimal snapshot fields
        builder.Property(oi => oi.WoodUnitPriceSnapshot).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.MaterialCostSnapshot).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.CarpenterAmountSnapshot).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.CompanyProfitSnapshot).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.SellingPrice).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.CarpenterPercentageSnapshot).HasColumnType("decimal(5,4)");
        builder.Property(oi => oi.CompanyProfitPercentageSnapshot).HasColumnType("decimal(5,4)");

        // ProductConfiguration reference (for traceability — not relied on for pricing)
        builder.HasOne(oi => oi.ProductConfiguration)
            .WithMany(cfg => cfg.OrderItems)
            .HasForeignKey(oi => oi.ProductConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:1 ProductionJob (each OrderItem generates exactly one ProductionJob)
        builder.HasOne(oi => oi.ProductionJob)
            .WithOne(pj => pj.OrderItem)
            .HasForeignKey<ProductionJob>(pj => pj.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}