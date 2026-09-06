namespace ECommerce.Application.Features.Customization.DTOs;

public record PriceBreakdownDto(
    int ProductId,
    string ProductName,
    int WoodMaterialId,
    string WoodMaterialName,
    int WoodColorId,
    string WoodColorName,
    decimal RequiredWoodSheets,
    decimal WoodUnitPrice,
    decimal WoodCost,
    decimal PaintCost,
    decimal ComponentCost,
    decimal MaterialCost,
    decimal CarpenterPercentage,
    decimal CarpenterAmount,
    decimal CompanyProfitPercentage,
    decimal CompanyProfit,
    decimal SellingPrice,
    decimal DepositPercentage,
    decimal DepositAmount,
    Dictionary<string, decimal> DimensionsSnapshot);
