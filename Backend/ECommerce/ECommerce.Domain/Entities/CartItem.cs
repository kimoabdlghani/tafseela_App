using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// A single item in the customer's cart, linked to a ProductConfiguration.
    /// The cart item becomes INVALID if the product or materials are deactivated
    /// before checkout. Validation happens at checkout time.
    /// </summary>
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int ProductConfigurationId { get; set; }
        public ProductConfiguration ProductConfiguration { get; set; } = null!;

        public DateTime AddedAt { get; set; }
    }
}
