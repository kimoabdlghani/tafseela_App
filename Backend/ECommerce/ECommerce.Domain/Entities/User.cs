using Microsoft.AspNetCore.Identity;
using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class User : IdentityUser<int>, ISoftDeletable
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        // Carpenter-specific (null for Admin/Customer)
        public CarpenterProfile? CarpenterProfile { get; set; }
        public ICollection<CarpenterApplication> CarpenterApplications { get; set; } = [];

        // Customer-specific (null for Admin/Carpenter)
        public Cart? Cart { get; set; }
    }
}
