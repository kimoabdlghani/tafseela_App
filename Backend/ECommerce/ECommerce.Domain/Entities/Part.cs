using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Master catalog of manufacturable parts (e.g., "Side Panel", "Door", "Shelf").
    /// Parts are what get manufactured from wood material.
    /// </summary>
    public class Part : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ProductPart> ProductParts { get; set; } = [];
    }
}
