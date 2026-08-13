using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity, ISoftDeletable
    {
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Category Category { get; set; } = null!;
        public Brand? Brand { get; set; }

        public ICollection<Measurement> Measurements { get; set; } = [];
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<ProductVariant> Variants { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
