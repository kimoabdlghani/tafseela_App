using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// Associates a Part with a Product and defines the base quantity needed.
    /// Note: Part dimension rules (how Part dimensions derive from Product dimensions)
    /// are a PENDING design decision and will be implemented in a future phase.
    /// </summary>
    public class ProductPart : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int PartId { get; set; }
        public Part Part { get; set; } = null!;

        public int Quantity { get; set; }
        public int DisplayOrder { get; set; }
    }
}
