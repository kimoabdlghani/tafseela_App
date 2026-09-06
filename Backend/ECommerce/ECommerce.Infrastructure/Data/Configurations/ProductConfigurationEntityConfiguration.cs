using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductConfigEntity = ECommerce.Domain.Entities.ProductConfiguration;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductConfigurationEntityConfiguration : IEntityTypeConfiguration<ProductConfigEntity>
{
    public void Configure(EntityTypeBuilder<ProductConfigEntity> builder)
    {
        builder.HasKey(cfg => cfg.Id);

        // WoodMaterial reference
        builder.HasOne(cfg => cfg.WoodMaterial)
            .WithMany()
            .HasForeignKey(cfg => cfg.WoodMaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        // WoodColor reference
        builder.HasOne(cfg => cfg.WoodColor)
            .WithMany()
            .HasForeignKey(cfg => cfg.WoodColorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Dimension values
        builder.HasMany(cfg => cfg.Dimensions)
            .WithOne(cd => cd.ProductConfiguration)
            .HasForeignKey(cd => cd.ProductConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
