using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Customization.Commands.SaveConfiguration;

public record SaveProductConfigurationCommand(
    int ProductId,
    int WoodMaterialId,
    int WoodColorId,
    Dictionary<string, decimal> Dimensions) : IRequest<Result<int>>;

public class SaveProductConfigurationCommandValidator : AbstractValidator<SaveProductConfigurationCommand>
{
    public SaveProductConfigurationCommandValidator()
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

public class SaveProductConfigurationCommandHandler : IRequestHandler<SaveProductConfigurationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;

    public SaveProductConfigurationCommandHandler(
        IApplicationDbContext context,
        IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<Result<int>> Handle(
        SaveProductConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Re-validate the configuration through PricingService
        var priceResult = await _pricingService.CalculatePriceAsync(
            request.ProductId,
            request.WoodMaterialId,
            request.WoodColorId,
            request.Dimensions,
            cancellationToken);

        if (!priceResult.IsSuccess)
        {
            return Result<int>.Failure(priceResult.Error);
        }

        var product = await _context.Products
            .Include(p => p.Dimensions)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result<int>.Failure("Product not found.");
        }

        // 2. Create the ProductConfiguration entity
        var configuration = new ProductConfiguration
        {
            ProductId = request.ProductId,
            WoodMaterialId = request.WoodMaterialId,
            WoodColorId = request.WoodColorId,
            Dimensions = new List<ConfigurationDimension>()
        };

        foreach (var dim in product.Dimensions)
        {
            if (!request.Dimensions.TryGetValue(dim.Name, out var value))
            {
                value = dim.DefaultValue ?? dim.MinValue;
            }

            configuration.Dimensions.Add(new ConfigurationDimension
            {
                ProductDimensionId = dim.Id,
                Value = value
            });
        }

        _context.ProductConfigurations.Add(configuration);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(configuration.Id);
    }
}
