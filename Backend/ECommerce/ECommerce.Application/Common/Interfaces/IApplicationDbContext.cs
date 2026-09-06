using Microsoft.EntityFrameworkCore;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        // ─── Identity ────────────────────────────────────────────────────────
        DbSet<User> Users { get; }
        DbSet<RefreshToken> RefreshTokens { get; }

        // ─── Location ────────────────────────────────────────────────────────
        DbSet<Address> Addresses { get; }

        // ─── Catalog ─────────────────────────────────────────────────────────
        DbSet<Category> Categories { get; }
        DbSet<Product> Products { get; }
        DbSet<ProductImage> ProductImages { get; }
        DbSet<ProductDimension> ProductDimensions { get; }

        // ─── Materials ───────────────────────────────────────────────────────
        DbSet<WoodMaterial> WoodMaterials { get; }
        DbSet<WoodColor> WoodColors { get; }
        DbSet<ProductWoodMaterial> ProductWoodMaterials { get; }

        // ─── Manufacturing ───────────────────────────────────────────────────
        DbSet<Part> Parts { get; }
        DbSet<ProductPart> ProductParts { get; }
        DbSet<Component> Components { get; }
        DbSet<ProductComponent> ProductComponents { get; }
        DbSet<Paint> Paints { get; }

        // ─── Configuration ───────────────────────────────────────────────────
        DbSet<ProductConfiguration> ProductConfigurations { get; }
        DbSet<ConfigurationDimension> ConfigurationDimensions { get; }

        // ─── Carpenter ───────────────────────────────────────────────────────
        DbSet<CarpenterProfile> CarpenterProfiles { get; }
        DbSet<CarpenterApplication> CarpenterApplications { get; }

        // ─── Cart ────────────────────────────────────────────────────────────
        DbSet<Cart> Carts { get; }
        DbSet<CartItem> CartItems { get; }

        // ─── Orders ──────────────────────────────────────────────────────────
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Payment> Payments { get; }

        // ─── Production ──────────────────────────────────────────────────────
        DbSet<ProductionJob> ProductionJobs { get; }
        DbSet<ProductionJobStatusHistory> ProductionJobStatusHistories { get; }

        // ─── Settings ────────────────────────────────────────────────────────
        DbSet<CompanySettings> CompanySettings { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}