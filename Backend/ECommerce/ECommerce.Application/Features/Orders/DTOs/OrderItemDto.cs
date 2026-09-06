namespace ECommerce.Application.Features.Orders.DTOs;

public record OrderItemDto(
    int Id,
    int ProductConfigurationId,
    string ProductName,
    string WoodMaterialName,
    string WoodColorName,
    string DimensionsSnapshot,
    string ComponentsSnapshot,
    decimal MaterialCost,
    decimal CarpenterAmount,
    decimal CompanyProfit,
    decimal SellingPrice,
    int Quantity,
    decimal TotalPrice,
    string? ProductionJobStatus,
    int? ProductionJobId);