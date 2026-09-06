using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Customization.DTOs;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Customization.Queries.CalculatePrice;

public record CalculateProductPriceQuery(
    int ProductId,
    int WoodMaterialId,
    int WoodColorId,
    Dictionary<string, decimal> Dimensions) : IRequest<Result<PriceBreakdownDto>>;

public class CalculateProductPriceQueryValidator : AbstractValidator<CalculateProductPriceQuery>
{
    public CalculateProductPriceQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be a positive number.");

        RuleFor(x => x.WoodMaterialId)
            .GreaterThan(0).WithMessage("WoodMaterialId must be a positive number.");

        RuleFor(x => x.WoodColorId)
            .GreaterThan(0).WithMessage("WoodColorId must be a positive number.");

        RuleFor(x => x.Dimensions)
            .NotNull().WithMessage("Dimensions dictionary is required.");
    }
}

public class CalculateProductPriceQueryHandler : IRequestHandler<CalculateProductPriceQuery, Result<PriceBreakdownDto>>
{
    private readonly IPricingService _pricingService;

    public CalculateProductPriceQueryHandler(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public async Task<Result<PriceBreakdownDto>> Handle(
        CalculateProductPriceQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _pricingService.CalculatePriceAsync(
            request.ProductId,
            request.WoodMaterialId,
            request.WoodColorId,
            request.Dimensions,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PriceBreakdownDto>.Failure(result.Error);
        }

        var b = result.Data;
        var dto = new PriceBreakdownDto(
            b.ProductId,
            b.ProductName,
            b.WoodMaterialId,
            b.WoodMaterialName,
            b.WoodColorId,
            b.WoodColorName,
            b.RequiredWoodSheets,
            b.WoodUnitPrice,
            b.WoodCost,
            b.PaintCost,
            b.ComponentCost,
            b.MaterialCost,
            b.CarpenterPercentage,
            b.CarpenterAmount,
            b.CompanyProfitPercentage,
            b.CompanyProfit,
            b.SellingPrice,
            b.DepositPercentage,
            b.DepositAmount,
            b.DimensionsSnapshot);

        return Result<PriceBreakdownDto>.Success(dto);
    }
}
