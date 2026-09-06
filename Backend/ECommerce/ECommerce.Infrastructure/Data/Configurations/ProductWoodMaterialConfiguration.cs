using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductWoodMaterialConfiguration : IEntityTypeConfiguration<ProductWoodMaterial>
{
    public void Configure(EntityTypeBuilder<ProductWoodMaterial> builder)
    {
        // Composite primary key
        builder.HasKey(pwm => new { pwm.ProductId, pwm.WoodMaterialId });
    }
}
