using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Represents a complete wood material (e.g., "MDF 18mm", "Solid Pine").
    /// The thickness is encoded in the name and the Thickness field separately.
    /// Each WoodMaterial is an independent choice for the customer — they do NOT
    /// choose wood type and thickness separately.
    /// </summary>
    public class WoodMaterial : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>Thickness in millimeters (e.g., 18, 25)</summary>
        public decimal Thickness { get; set; }

        public string Unit { get; set; } = "mm";

        /// <summary>Price per sheet/unit</summary>
        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<WoodColor> Colors { get; set; } = [];
        public ICollection<ProductWoodMaterial> ProductWoodMaterials { get; set; } = [];
    }
}
