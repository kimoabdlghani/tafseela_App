using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CarpenterApplicationConfiguration : IEntityTypeConfiguration<CarpenterApplication>
{
    public void Configure(EntityTypeBuilder<CarpenterApplication> builder)
    {
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ca => ca.ReviewNotes)
            .HasMaxLength(1000);

        // The admin who reviewed (self-referencing to User, different FK name)
        builder.HasOne(ca => ca.ReviewedByAdmin)
            .WithMany()
            .HasForeignKey(ca => ca.ReviewedByAdminId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
