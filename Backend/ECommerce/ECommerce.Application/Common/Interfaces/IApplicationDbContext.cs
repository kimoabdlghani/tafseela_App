using Microsoft.EntityFrameworkCore;
using ECommerce.Domain.Entities;
namespace ECommerce.Application.Common.Interfaces
{
    public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Address> Addresses { get; }
    
    DbSet<Category> Categories { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    
    DbSet<CartItem> CartItems { get; }
    DbSet<WishlistItem> WishlistItems { get; }
    
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Payment> Payments { get; }
    
    DbSet<Review> Reviews { get; }
    
    DbSet<Material> Materials { get; }
    DbSet<CustomOrderRequest> CustomOrderRequests { get; }
    DbSet<RequestAttachment> RequestAttachments { get; }
    DbSet<Quotation> Quotations { get; }
    DbSet<Measurement> Measurements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 
}