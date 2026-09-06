using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Immutable audit trail of every status change on a ProductionJob.
    /// Answers: who changed it, when, from/to which status, and any notes.
    /// </summary>
    public class ProductionJobStatusHistory : BaseEntity
    {
        public int ProductionJobId { get; set; }
        public ProductionJob ProductionJob { get; set; } = null!;

        public ProductionJobStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }

        public int ChangedByUserId { get; set; }
        public User ChangedByUser { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
