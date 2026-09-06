using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Paint/finish consumable material.
    /// Paint quantity calculation rules are a PENDING design decision.
    /// </summary>
    public class Paint : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = "Liter";
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
