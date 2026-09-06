using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductionJobConfiguration : IEntityTypeConfiguration<ProductionJob>
{
    public void Configure(EntityTypeBuilder<ProductionJob> builder)
    {
        builder.HasKey(pj => pj.Id);

        builder.Property(pj => pj.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(pj => pj.ReworkNotes)
            .HasMaxLength(1000);

        builder.Property(pj => pj.ProductionSnapshot)
            .IsRequired()
            .HasMaxLength(8000);

        // StatusHistory
        builder.HasMany(pj => pj.StatusHistory)
            .WithOne(h => h.ProductionJob)
            .HasForeignKey(h => h.ProductionJobId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChangedByUser in StatusHistory — configured there
    }
}
