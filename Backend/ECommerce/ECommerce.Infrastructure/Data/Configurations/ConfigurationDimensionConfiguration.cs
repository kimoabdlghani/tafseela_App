using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ConfigurationDimensionConfiguration : IEntityTypeConfiguration<ConfigurationDimension>
{
    public void Configure(EntityTypeBuilder<ConfigurationDimension> builder)
    {
        builder.HasKey(cd => cd.Id);

        builder.Property(cd => cd.Value).HasColumnType("decimal(10,2)");
    }
}
