using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity, ISoftDeletable
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Draft = Admin hasn't published yet. Published = Visible to customers.
        /// </summary>
        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        /// <summary>
        /// IsActive = false means the product is soft-deactivated (hidden from customers).
        /// A Published + Inactive product is no longer available for new orders.
        /// </summary>
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<ProductDimension> Dimensions { get; set; } = [];
        public ICollection<ProductWoodMaterial> ProductWoodMaterials { get; set; } = [];
        public ICollection<ProductPart> ProductParts { get; set; } = [];
        public ICollection<ProductComponent> ProductComponents { get; set; } = [];
        public ICollection<ProductConfiguration> Configurations { get; set; } = [];
    }
}
