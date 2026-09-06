using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class PaintConfiguration : IEntityTypeConfiguration<Paint>
{
    public void Configure(EntityTypeBuilder<Paint> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Unit).IsRequired().HasMaxLength(20);
        builder.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
