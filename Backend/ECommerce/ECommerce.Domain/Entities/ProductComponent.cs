using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Defines which Components a Product requires and in what quantity.
    /// In V1, Quantity is fixed. Future versions may support formula-based quantities.
    /// </summary>
    public class ProductComponent : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int ComponentId { get; set; }
        public Component Component { get; set; } = null!;

        /// <summary>Fixed quantity per product unit (e.g., 6 hinges per wardrobe)</summary>
        public int Quantity { get; set; }
    }
}
