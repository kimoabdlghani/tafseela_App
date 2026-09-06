using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Defines a customizable dimension for a product (e.g., Width, Height, Depth).
    /// Admin sets the allowed range; customer inputs a value within that range.
    /// </summary>
    public class ProductDimension : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        /// <summary>Dimension label (e.g., "Width", "Height", "Depth")</summary>
        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = "cm";
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        /// <summary>Default value shown to customer (must be within Min/Max)</summary>
        public decimal? DefaultValue { get; set; }

        public int DisplayOrder { get; set; }

        public ICollection<ConfigurationDimension> ConfigurationDimensions { get; set; } = [];
    }
}
