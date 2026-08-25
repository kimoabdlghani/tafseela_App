using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class Review : BaseEntity, ISoftDeletable
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
