using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity, ISoftDeletable
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Estimated starting price for customer reference only
        public decimal? EstimatedStartingPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<Measurement> Measurements { get; set; } = [];
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
