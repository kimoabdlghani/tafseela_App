using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Online;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// Unique reference from the payment gateway (e.g. Stripe PaymentIntent ID).
        /// Used for idempotency and webhook verification.
        /// </summary>
        public string? GatewayTransactionId { get; set; }

        public DateTime? PaidAt { get; set; }
        public string? Notes { get; set; }
    }
}
