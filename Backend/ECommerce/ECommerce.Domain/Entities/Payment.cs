using ECommerce.Domain.Enums;
using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string? StripeClientSecret { get; set; }
        public PaymentMilestoneType MilestoneType { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
