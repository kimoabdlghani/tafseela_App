using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Description).HasMaxLength(500);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasMany(p => p.ProductParts)
            .WithOne(pp => pp.Part)
            .HasForeignKey(pp => pp.PartId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
