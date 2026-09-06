using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class WoodColorConfiguration : IEntityTypeConfiguration<WoodColor>
{
    public void Configure(EntityTypeBuilder<WoodColor> builder)
    {
        builder.HasKey(wc => wc.Id);

        builder.Property(wc => wc.Name).IsRequired().HasMaxLength(100);

        builder.HasQueryFilter(wc => !wc.IsDeleted);
    }
}
