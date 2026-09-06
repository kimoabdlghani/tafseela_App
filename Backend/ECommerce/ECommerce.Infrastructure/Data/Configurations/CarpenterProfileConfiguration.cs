using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CarpenterProfileConfiguration : IEntityTypeConfiguration<CarpenterProfile>
{
    public void Configure(EntityTypeBuilder<CarpenterProfile> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        // Unique: each user has at most one carpenter profile
        builder.HasIndex(cp => cp.UserId).IsUnique();

        // AssignedJobs
        builder.HasMany(cp => cp.AssignedJobs)
            .WithOne(pj => pj.AssignedCarpenter)
            .HasForeignKey(pj => pj.AssignedCarpenterId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
