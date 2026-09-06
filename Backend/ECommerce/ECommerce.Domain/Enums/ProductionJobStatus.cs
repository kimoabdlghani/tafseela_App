namespace ECommerce.Domain.Enums
{
    public enum ProductionJobStatus
    {
        Available = 0,
        Assigned = 1,
        InProduction = 2,
        CarpenterCompleted = 3,
        ReworkRequired = 4,
        Ready = 5,
        Shipping = 6,
        Delivered = 7,
        Cancelled = 8
    }
}
