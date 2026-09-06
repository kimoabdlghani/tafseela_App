using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Extended profile for users with the Carpenter role.
    /// Does NOT exist for Admin or Customer users.
    /// </summary>
    public class CarpenterProfile : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        /// <summary>
        /// Maximum number of active production jobs the carpenter can hold simultaneously.
        /// Starts at 1 for new carpenters; Admin can increase based on performance.
        /// </summary>
        public int MaxActiveJobs { get; set; } = 1;

        public CarpenterStatus Status { get; set; } = CarpenterStatus.PendingApproval;
        public DateTime JoinedAt { get; set; }

        public ICollection<ProductionJob> AssignedJobs { get; set; } = [];
    }
}
