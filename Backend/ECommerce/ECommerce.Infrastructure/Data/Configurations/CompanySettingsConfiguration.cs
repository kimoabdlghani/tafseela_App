using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
{
    public void Configure(EntityTypeBuilder<CompanySettings> builder)
    {
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.CarpenterPercentage).HasColumnType("decimal(5,4)");
        builder.Property(cs => cs.CompanyProfitPercentage).HasColumnType("decimal(5,4)");
        builder.Property(cs => cs.DepositPercentage).HasColumnType("decimal(5,4)");
    }
}
