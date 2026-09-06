using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// A color option belonging to a specific WoodMaterial.
    /// Customer first selects WoodMaterial, then sees only colors that belong to it.
    /// </summary>
    public class WoodColor : BaseEntity, ISoftDeletable
    {
        public int WoodMaterialId { get; set; }
        public WoodMaterial WoodMaterial { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
