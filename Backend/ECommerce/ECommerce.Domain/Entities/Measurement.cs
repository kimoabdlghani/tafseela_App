using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class Measurement : BaseEntity, ISoftDeletable
    {
        public int ProductId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        public Product Product { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
