using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Unit).IsRequired().HasMaxLength(20);
        builder.Property(c => c.UnitPrice).HasColumnType("decimal(18,2)");

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasMany(c => c.ProductComponents)
            .WithOne(pc => pc.Component)
            .HasForeignKey(pc => pc.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
