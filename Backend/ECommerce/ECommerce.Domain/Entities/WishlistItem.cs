using ECommerce.Domain.Common;
namespace ECommerce.Domain.Entities
{
    public class WishlistItem : BaseEntity
    {
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
