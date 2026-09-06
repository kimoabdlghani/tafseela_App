using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;

namespace ECommerce.Infrastructure.Services;

public class CalculationEngine : ICalculationEngine
{
    // Standard wood sheet dimensions: 1.22m x 2.44m = ~2.9768 m²
    private const decimal StandardSheetAreaM2 = 2.9768m;
    private const decimal DefaultWasteFactor = 0.15m; // 15% cutting & assembly waste

    public WoodRequirementResult CalculateWoodRequirements(IReadOnlyDictionary<string, decimal> dimensions)
    {
        var surfaceAreaM2 = CalculateSurfaceAreaM2(dimensions);

        // Effective area including waste factor
        var effectiveAreaM2 = surfaceAreaM2 * (1m + DefaultWasteFactor);

        // Calculate sheets required; in carpentry, sheets are whole units (minimum 1 sheet)
        var rawSheets = effectiveAreaM2 / StandardSheetAreaM2;
        var requiredSheets = Math.Max(1m, Math.Ceiling(rawSheets));

        return new WoodRequirementResult(
            RequiredSheets: requiredSheets,
            TotalSurfaceAreaM2: Math.Round(surfaceAreaM2, 4),
            WasteFactorPercentage: DefaultWasteFactor * 100m);
    }

    public decimal CalculateSurfaceAreaM2(IReadOnlyDictionary<string, decimal> dimensions)
    {
        if (dimensions == null || dimensions.Count == 0)
        {
            return 1.0m; // Default baseline surface area in m²
        }

        // Normalize dimension keys (case-insensitive)
        var normalized = dimensions.ToDictionary(
            k => k.Key.Trim().ToLowerInvariant(),
            v => Math.Max(0.01m, v.Value));

        decimal width = FindDimension(normalized, "width", "w", "عرض", "العرض");
        decimal height = FindDimension(normalized, "height", "h", "ارتفاع", "الارتفاع");
        decimal depth = FindDimension(normalized, "depth", "d", "عمق", "العمق", "length", "l", "طول", "الطول");

        // Convert cm to meters: divide cm by 100, so cm * cm / 10000 = m²
        decimal surfaceAreaM2;

        if (width > 0 && height > 0 && depth > 0)
        {
            // 3D Box/Furniture surface area (all 6 faces)
            surfaceAreaM2 = 2m * ((width * height) + (width * depth) + (height * depth)) / 10000m;
        }
        else if (width > 0 && height > 0)
        {
            // 2D 2-sided panel
            surfaceAreaM2 = 2m * (width * height) / 10000m;
        }
        else
        {
            // Sum available dimensions as a fallback
            var values = normalized.Values.ToList();
            if (values.Count >= 2)
            {
                surfaceAreaM2 = 2m * (values[0] * values[1]) / 10000m;
            }
            else
            {
                surfaceAreaM2 = (values[0] * values[0]) / 10000m;
            }
        }

        return Math.Max(0.1m, Math.Round(surfaceAreaM2, 4));
    }

    private static decimal FindDimension(Dictionary<string, decimal> dict, params string[] candidateKeys)
    {
        foreach (var key in candidateKeys)
        {
            if (dict.TryGetValue(key, out var val) && val > 0)
            {
                return val;
            }
        }
        return 0m;
    }
}
