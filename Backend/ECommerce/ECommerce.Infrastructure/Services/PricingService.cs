using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class PricingService : IPricingService
{
    private readonly IApplicationDbContext _context;
    private readonly ICalculationEngine _calculationEngine;

    // Fallback percentages if no CompanySettings exist in the database
    private const decimal DefaultCarpenterPercentage = 0.30m;     // 30%
    private const decimal DefaultCompanyProfitPercentage = 0.20m; // 20%
    private const decimal DefaultDepositPercentage = 0.30m;       // 30%

    public PricingService(IApplicationDbContext context, ICalculationEngine calculationEngine)
    {
        _context = context;
        _calculationEngine = calculationEngine;
    }

    public async Task<Result<PriceBreakdown>> CalculatePriceAsync(
        int productId,
        int woodMaterialId,
        int woodColorId,
        IReadOnlyDictionary<string, decimal> dimensions,
        CancellationToken cancellationToken = default)
    {
        // 1. Fetch Product with Dimensions and Components
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Dimensions)
            .Include(p => p.ProductWoodMaterials)
            .Include(p => p.ProductComponents)
                .ThenInclude(pc => pc.Component)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product == null)
        {
            return Result<PriceBreakdown>.Failure($"Product with ID {productId} was not found.");
        }

        if (product.Status != ProductStatus.Published || !product.IsActive)
        {
            return Result<PriceBreakdown>.Failure($"Product '{product.Name}' is not currently available for ordering.");
        }

        // 2. Validate WoodMaterial association with Product
        var isWoodAllowed = product.ProductWoodMaterials.Any(pwm => pwm.WoodMaterialId == woodMaterialId);
        if (!isWoodAllowed)
        {
            return Result<PriceBreakdown>.Failure($"The selected wood material is not allowed for this product.");
        }

        var woodMaterial = await _context.WoodMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(wm => wm.Id == woodMaterialId, cancellationToken);

        if (woodMaterial == null || !woodMaterial.IsActive)
        {
            return Result<PriceBreakdown>.Failure($"Selected wood material is inactive or not found.");
        }

        // 3. Validate WoodColor belongs to WoodMaterial
        var woodColor = await _context.WoodColors
            .AsNoTracking()
            .FirstOrDefaultAsync(wc => wc.Id == woodColorId && wc.WoodMaterialId == woodMaterialId, cancellationToken);

        if (woodColor == null || !woodColor.IsActive)
        {
            return Result<PriceBreakdown>.Failure($"Selected color is inactive or does not belong to the selected wood material.");
        }

        // 4. Validate Dimensions against Product Dimension Rules (Min/Max)
        var dimensionsSnapshot = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var def in product.Dimensions)
        {
            // Look up dimension value from request by dimension name or id
            if (!dimensions.TryGetValue(def.Name, out var value))
            {
                // Fallback to default value or min value if dimension not supplied
                value = def.DefaultValue ?? def.MinValue;
            }

            if (value < def.MinValue || value > def.MaxValue)
            {
                return Result<PriceBreakdown>.Failure(
                    $"Dimension '{def.Name}' value {value} is out of allowable range [{def.MinValue} - {def.MaxValue}] {def.Unit}.");
            }

            dimensionsSnapshot[def.Name] = value;
        }

        // 5. Calculate Wood Requirement
        var woodResult = _calculationEngine.CalculateWoodRequirements(dimensionsSnapshot);
        var woodCost = Math.Round(woodResult.RequiredSheets * woodMaterial.UnitPrice, 2);

        // 6. Calculate Component Costs
        decimal componentCost = 0m;
        foreach (var pc in product.ProductComponents)
        {
            if (pc.Component != null && pc.Component.IsActive)
            {
                componentCost += pc.Quantity * pc.Component.UnitPrice;
            }
        }
        componentCost = Math.Round(componentCost, 2);

        // 7. Paint Cost (Optional consumable, default 0 in V1)
        decimal paintCost = 0m;

        // 8. Total Material Cost
        var materialCost = Math.Round(woodCost + componentCost + paintCost, 2);

        // 9. Load Active Company Settings for Percentages
        var settings = await _context.CompanySettings
            .AsNoTracking()
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        var carpenterPercentage = settings?.CarpenterPercentage ?? DefaultCarpenterPercentage;
        var companyProfitPercentage = settings?.CompanyProfitPercentage ?? DefaultCompanyProfitPercentage;
        var depositPercentage = settings?.DepositPercentage ?? DefaultDepositPercentage;

        // 10. Financial Calculations
        var carpenterAmount = Math.Round(materialCost * carpenterPercentage, 2);
        var companyProfit = Math.Round(materialCost * companyProfitPercentage, 2);
        var sellingPrice = Math.Round(materialCost + carpenterAmount + companyProfit, 2);
        var depositAmount = Math.Round(sellingPrice * depositPercentage, 2);

        var breakdown = new PriceBreakdown(
            ProductId: product.Id,
            ProductName: product.Name,
            WoodMaterialId: woodMaterial.Id,
            WoodMaterialName: woodMaterial.Name,
            WoodColorId: woodColor.Id,
            WoodColorName: woodColor.Name,
            RequiredWoodSheets: woodResult.RequiredSheets,
            WoodUnitPrice: woodMaterial.UnitPrice,
            WoodCost: woodCost,
            PaintCost: paintCost,
            ComponentCost: componentCost,
            MaterialCost: materialCost,
            CarpenterPercentage: carpenterPercentage,
            CarpenterAmount: carpenterAmount,
            CompanyProfitPercentage: companyProfitPercentage,
            CompanyProfit: companyProfit,
            SellingPrice: sellingPrice,
            DepositPercentage: depositPercentage,
            DepositAmount: depositAmount,
            DimensionsSnapshot: dimensionsSnapshot);

        return Result<PriceBreakdown>.Success(breakdown);
    }
}
