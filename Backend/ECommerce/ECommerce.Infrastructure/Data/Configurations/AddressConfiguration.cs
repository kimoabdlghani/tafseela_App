using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Street).IsRequired().HasMaxLength(300);
        builder.Property(a => a.Building).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Apartment).HasMaxLength(50);
        builder.Property(a => a.Floor).HasMaxLength(20);
        builder.Property(a => a.PostalCode).HasMaxLength(20);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}