using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductDimensionConfiguration : IEntityTypeConfiguration<ProductDimension>
{
    public void Configure(EntityTypeBuilder<ProductDimension> builder)
    {
        builder.HasKey(pd => pd.Id);

        builder.Property(pd => pd.Name).IsRequired().HasMaxLength(100);
        builder.Property(pd => pd.Unit).IsRequired().HasMaxLength(20);
        builder.Property(pd => pd.MinValue).HasColumnType("decimal(10,2)");
        builder.Property(pd => pd.MaxValue).HasColumnType("decimal(10,2)");
        builder.Property(pd => pd.DefaultValue).HasColumnType("decimal(10,2)");

        builder.HasMany(pd => pd.ConfigurationDimensions)
            .WithOne(cd => cd.ProductDimension)
            .HasForeignKey(cd => cd.ProductDimensionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
