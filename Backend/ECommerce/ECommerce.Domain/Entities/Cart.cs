using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// A customer's shopping cart. One cart per customer (1:1 with User).
    /// The cart is NOT an Order — it becomes an Order only after checkout.
    /// </summary>
    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<CartItem> Items { get; set; } = [];
    }
}
