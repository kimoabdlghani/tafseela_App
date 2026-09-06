using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Represents a specific customization choice made by a customer for a product.
    /// It captures WHAT the customer selected (Wood + Color).
    /// Dimension values are stored in ConfigurationDimension (separate rows).
    /// Price is NOT stored here — it is calculated at checkout time.
    /// </summary>
    public class ProductConfiguration : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int WoodMaterialId { get; set; }
        public WoodMaterial WoodMaterial { get; set; } = null!;

        public int WoodColorId { get; set; }
        public WoodColor WoodColor { get; set; } = null!;

        public ICollection<ConfigurationDimension> Dimensions { get; set; } = [];
        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
