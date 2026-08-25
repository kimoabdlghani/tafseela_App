using Microsoft.AspNetCore.Identity;
using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class User : IdentityUser<int>, ISoftDeletable
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserStatus UserStatus { get; set; } = UserStatus.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<WishlistItem> WishlistItems { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
