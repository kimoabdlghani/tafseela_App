namespace ECommerce.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        DepositPaid = 1,
        InProduction = 2,
        CarpenterCompleted = 3,
        AdminInspection = 4,
        Ready = 5,
        Shipping = 6,
        Delivered = 7,
        Cancelled = 8
    }
}