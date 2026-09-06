using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Represents one line in an order. Holds full snapshot of what was ordered
    /// so that future price/material changes do NOT affect historical orders.
    /// </summary>
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // Reference to the original configuration (kept for traceability, not relied on for pricing)
        public int ProductConfigurationId { get; set; }
        public ProductConfiguration ProductConfiguration { get; set; } = null!;

        // === Snapshot fields — all values frozen at checkout time ===

        // Product snapshot
        public string ProductNameSnapshot { get; set; } = string.Empty;
        public string ProductDescriptionSnapshot { get; set; } = string.Empty;

        // Wood snapshot
        public string WoodMaterialNameSnapshot { get; set; } = string.Empty;
        public string WoodColorNameSnapshot { get; set; } = string.Empty;
        public decimal WoodUnitPriceSnapshot { get; set; }

        // Dimensions snapshot (JSON: [{ "name": "Width", "value": 240, "unit": "cm" }, ...])
        public string DimensionsSnapshot { get; set; } = string.Empty;

        // Components snapshot (JSON: [{ "name": "Hinge", "qty": 6, "unitPrice": 50 }, ...])
        public string ComponentsSnapshot { get; set; } = string.Empty;

        // Pricing snapshot
        public decimal MaterialCostSnapshot { get; set; }
        public decimal CarpenterAmountSnapshot { get; set; }
        public decimal CompanyProfitSnapshot { get; set; }
        public decimal SellingPrice { get; set; }

        // Percentage snapshots (for reporting and transparency)
        public decimal CarpenterPercentageSnapshot { get; set; }
        public decimal CompanyProfitPercentageSnapshot { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal TotalPrice { get; set; }

        public ProductionJob? ProductionJob { get; set; }
    }
}
