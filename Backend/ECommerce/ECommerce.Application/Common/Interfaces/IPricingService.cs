using ECommerce.Application.Common.Models;

namespace ECommerce.Application.Common.Interfaces;

public interface IPricingService
{
    /// <summary>
    /// Calculates the full price breakdown for a specific product configuration,
    /// reading current material unit prices, component requirements, and company settings.
    /// </summary>
    Task<Result<PriceBreakdown>> CalculatePriceAsync(
        int productId,
        int woodMaterialId,
        int woodColorId,
        IReadOnlyDictionary<string, decimal> dimensions,
        CancellationToken cancellationToken = default);
}
