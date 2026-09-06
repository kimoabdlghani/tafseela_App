namespace ECommerce.Application.Common.Models;

public record WoodRequirementResult(
    decimal RequiredSheets,
    decimal TotalSurfaceAreaM2,
    decimal WasteFactorPercentage);

public record PriceBreakdown(
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
