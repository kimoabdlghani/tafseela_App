using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// System-wide settings controlled by Admin.
    /// A new row is inserted whenever settings change, preserving history.
    /// Orders always snapshot the settings at time of purchase — old orders
    /// are NOT affected by future settings changes.
    /// </summary>
    public class CompanySettings : BaseEntity
    {
        /// <summary>Percentage of material cost paid to the carpenter (e.g., 0.20 = 20%)</summary>
        public decimal CarpenterPercentage { get; set; }

        /// <summary>Percentage of material cost kept as company profit (e.g., 0.30 = 30%)</summary>
        public decimal CompanyProfitPercentage { get; set; }

        /// <summary>Percentage of order total required as deposit (e.g., 0.50 = 50%)</summary>
        public decimal DepositPercentage { get; set; }

        /// <summary>Timestamp from which these settings are effective</summary>
        public DateTime EffectiveFrom { get; set; }
    }
}
