using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasQueryFilter(p => !p.IsDeleted);

        // Category relationship
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Images
        builder.HasMany(p => p.Images)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Dimensions
        builder.HasMany(p => p.Dimensions)
            .WithOne(pd => pd.Product)
            .HasForeignKey(pd => pd.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductWoodMaterials (join)
        builder.HasMany(p => p.ProductWoodMaterials)
            .WithOne(pwm => pwm.Product)
            .HasForeignKey(pwm => pwm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductParts
        builder.HasMany(p => p.ProductParts)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductComponents
        builder.HasMany(p => p.ProductComponents)
            .WithOne(pc => pc.Product)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurations
        builder.HasMany(p => p.Configurations)
            .WithOne(cfg => cfg.Product)
            .HasForeignKey(cfg => cfg.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}