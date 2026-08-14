using ECommerce.Domain.Common;
namespace ECommerce.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; } = null!;
    }
}
