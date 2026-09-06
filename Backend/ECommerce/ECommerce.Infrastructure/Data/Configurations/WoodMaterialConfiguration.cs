using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class WoodMaterialConfiguration : IEntityTypeConfiguration<WoodMaterial>
{
    public void Configure(EntityTypeBuilder<WoodMaterial> builder)
    {
        builder.HasKey(wm => wm.Id);

        builder.Property(wm => wm.Name).IsRequired().HasMaxLength(150);
        builder.Property(wm => wm.Unit).IsRequired().HasMaxLength(20);
        builder.Property(wm => wm.Thickness).HasColumnType("decimal(8,2)");
        builder.Property(wm => wm.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasQueryFilter(wm => !wm.IsDeleted);

        builder.HasMany(wm => wm.Colors)
            .WithOne(wc => wc.WoodMaterial)
            .HasForeignKey(wc => wc.WoodMaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(wm => wm.ProductWoodMaterials)
            .WithOne(pwm => pwm.WoodMaterial)
            .HasForeignKey(pwm => pwm.WoodMaterialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
