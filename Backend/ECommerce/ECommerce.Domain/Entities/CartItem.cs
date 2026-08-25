using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;

        public int Quantity { get; set; } = 1;
    }
}
