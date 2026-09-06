using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Financial breakdown
        public decimal ProductTotal { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal DepositAmount { get; set; }

        // Settings snapshot — preserves values at time of order
        public decimal DepositPercentageSnapshot { get; set; }

        // Address snapshot — in case customer changes address later
        public string AddressSnapshot { get; set; } = string.Empty; // JSON

        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];
        public ICollection<ProductionJob> ProductionJobs { get; set; } = [];
    }
}
