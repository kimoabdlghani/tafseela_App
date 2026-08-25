using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities
{
    public class ProductVariant : BaseEntity, ISoftDeletable
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        /// <summary>
        /// اللون أو الخامة (مثال: "بني جوز", "أبيض لاكيه")
        /// </summary>
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// المقاس المتاح (مثال: "120×60×75 سم", "160×80×75 سم")
        /// </summary>
        public string Size { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    }
}
