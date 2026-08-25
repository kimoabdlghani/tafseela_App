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
    // user table not be dbset<user> because user table is in IdentityDbContext
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<CustomOrderRequest> CustomOrderRequests { get; set; }
    public DbSet<RequestAttachment> RequestAttachments { get; set; }
    public DbSet<Quotation> Quotations { get; set; }
    public DbSet<Measurement> Measurements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}