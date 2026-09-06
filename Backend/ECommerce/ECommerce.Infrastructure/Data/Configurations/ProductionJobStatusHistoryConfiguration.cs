using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductionJobStatusHistoryConfiguration : IEntityTypeConfiguration<ProductionJobStatusHistory>
{
    public void Configure(EntityTypeBuilder<ProductionJobStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(h => h.Notes)
            .HasMaxLength(1000);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
