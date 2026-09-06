using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Represents a single manufacturing unit derived from an OrderItem.
    /// One Order can produce multiple ProductionJobs (one per OrderItem).
    /// The job goes through its own lifecycle separate from the Order status.
    /// </summary>
    public class ProductionJob : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        /// <summary>Null until the Admin assigns a carpenter.</summary>
        public int? AssignedCarpenterId { get; set; }
        public CarpenterProfile? AssignedCarpenter { get; set; }

        public ProductionJobStatus Status { get; set; } = ProductionJobStatus.Available;

        // Timeline
        public DateTime? AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CarpenterCompletedAt { get; set; }
        public DateTime? AdminInspectedAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public string? ReworkNotes { get; set; }

        /// <summary>
        /// Snapshot of production requirements — what the carpenter needs to manufacture.
        /// Stored as JSON so the carpenter sees exact requirements without accessing formulas.
        /// </summary>
        public string ProductionSnapshot { get; set; } = string.Empty; // JSON

        public ICollection<ProductionJobStatusHistory> StatusHistory { get; set; } = [];
    }
}
