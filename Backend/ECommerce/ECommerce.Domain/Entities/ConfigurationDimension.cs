using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Stores the customer's chosen value for a specific dimension within a ProductConfiguration.
    /// One row per dimension per configuration (e.g., Width=240, Height=220, Depth=60).
    /// </summary>
    public class ConfigurationDimension : BaseEntity
    {
        public int ProductConfigurationId { get; set; }
        public ProductConfiguration ProductConfiguration { get; set; } = null!;

        public int ProductDimensionId { get; set; }
        public ProductDimension ProductDimension { get; set; } = null!;

        /// <summary>
        /// Value chosen by the customer — must be within ProductDimension.MinValue and MaxValue.
        /// </summary>
        public decimal Value { get; set; }
    }
}
