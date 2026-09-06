using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// A carpenter's request to join the platform.
    /// Kept as a separate entity (not a status on CarpenterProfile) to preserve
    /// the application history and allow multiple applications over time.
    /// </summary>
    public class CarpenterApplication : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public CarpenterApplicationStatus Status { get; set; } = CarpenterApplicationStatus.Pending;

        public DateTime SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        /// <summary>Admin who reviewed this application</summary>
        public int? ReviewedByAdminId { get; set; }
        public User? ReviewedByAdmin { get; set; }

        public string? ReviewNotes { get; set; }
    }
}
