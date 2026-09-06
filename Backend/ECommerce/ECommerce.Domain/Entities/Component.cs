using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Hardware/accessory component (e.g., Hinge, Handle, Drawer Slide).
    /// Components are assembled into the product, not manufactured from wood.
    /// </summary>
    public class Component : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = string.Empty;
        public ComponentType Type { get; set; }
        public string Unit { get; set; } = "pcs";
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ProductComponent> ProductComponents { get; set; } = [];
    }
}
