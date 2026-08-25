using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int AddressId { get; set; }

        public int? QuotationId { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;

        public User User { get; set; } = null!;
        public Quotation? Quotation { get; set; }
        public Address Address { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }

    public class Material : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePricePerSquareMeter { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<CustomOrderRequest> CustomOrderRequests { get; set; } = new List<CustomOrderRequest>();
    }

    public class RequestAttachment : BaseEntity
    {
        public int CustomOrderRequestId { get; set; }
        public CustomOrderRequest CustomOrderRequest { get; set; } = null!;

        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CustomOrderRequest : BaseEntity, ISoftDeletable
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? ReferenceProductId { get; set; }
        public Product? ReferenceProduct { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public decimal HeightCm { get; set; }
        public decimal WidthCm { get; set; }
        public decimal DepthCm { get; set; }

        public string FinishOrColor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? AdminNotes { get; set; }

        public CustomRequestStatus Status { get; set; } = CustomRequestStatus.Submitted;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<RequestAttachment> Attachments { get; set; } = new List<RequestAttachment>();
        public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    }

    public class Quotation : BaseEntity
    {
        public int CustomOrderRequestId { get; set; }
        public CustomOrderRequest CustomOrderRequest { get; set; } = null!;

        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal AccessoriesCost { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RequiredDepositAmount { get; set; }

        public int EstimatedProductionDays { get; set; }
        public DateTime ValidUntil { get; set; }
        public QuotationStatus Status { get; set; } = QuotationStatus.Draft;

        public string Notes { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
