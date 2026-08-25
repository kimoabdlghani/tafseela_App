using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity, ISoftDeletable
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Estimated starting price for customer reference only
        public decimal? EstimatedStartingPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = [];
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];

        // Measurements kept for reference (dimensions catalog)
        public ICollection<Measurement> Measurements { get; set; } = [];
    }
}
