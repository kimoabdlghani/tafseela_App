using ECommerce.Application.Common.Models;

namespace ECommerce.Application.Common.Interfaces;

public interface ICalculationEngine
{
    /// <summary>
    /// Calculates required wood sheets based on dimensions dictionary (Width, Height, Depth, etc.).
    /// Dimensions are expected in centimeters (cm).
    /// </summary>
    WoodRequirementResult CalculateWoodRequirements(IReadOnlyDictionary<string, decimal> dimensions);

    /// <summary>
    /// Calculates estimated surface area in square meters.
    /// </summary>
    decimal CalculateSurfaceAreaM2(IReadOnlyDictionary<string, decimal> dimensions);
}
