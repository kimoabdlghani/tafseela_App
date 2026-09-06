using System.Reflection;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ─── Identity (user table lives in IdentityDbContext) ───────────────────
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // ─── Location ────────────────────────────────────────────────────────────
    public DbSet<Address> Addresses { get; set; }

    // ─── Catalog ─────────────────────────────────────────────────────────────
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductDimension> ProductDimensions { get; set; }

    // ─── Materials ───────────────────────────────────────────────────────────
    public DbSet<WoodMaterial> WoodMaterials { get; set; }
    public DbSet<WoodColor> WoodColors { get; set; }
    public DbSet<ProductWoodMaterial> ProductWoodMaterials { get; set; }

    // ─── Manufacturing ───────────────────────────────────────────────────────
    public DbSet<Part> Parts { get; set; }
    public DbSet<ProductPart> ProductParts { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<ProductComponent> ProductComponents { get; set; }
    public DbSet<Paint> Paints { get; set; }

    // ─── Configuration ───────────────────────────────────────────────────────
    public DbSet<ProductConfiguration> ProductConfigurations { get; set; }
    public DbSet<ConfigurationDimension> ConfigurationDimensions { get; set; }

    // ─── Carpenter ───────────────────────────────────────────────────────────
    public DbSet<CarpenterProfile> CarpenterProfiles { get; set; }
    public DbSet<CarpenterApplication> CarpenterApplications { get; set; }

    // ─── Cart ────────────────────────────────────────────────────────────────
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    // ─── Orders ──────────────────────────────────────────────────────────────
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    // ─── Production ──────────────────────────────────────────────────────────
    public DbSet<ProductionJob> ProductionJobs { get; set; }
    public DbSet<ProductionJobStatusHistory> ProductionJobStatusHistories { get; set; }

    // ─── Settings ────────────────────────────────────────────────────────────
    public DbSet<CompanySettings> CompanySettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity table names
        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            if (entry.Entity is ISoftDeletable softDeletableEntity && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DeletedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}