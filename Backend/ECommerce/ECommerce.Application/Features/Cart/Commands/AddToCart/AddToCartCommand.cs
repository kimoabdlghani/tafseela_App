using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.Commands.AddToCart;

public record AddToCartCommand(
    int? ProductConfigurationId,
    int? ProductId,
    int? WoodMaterialId,
    int? WoodColorId,
    Dictionary<string, decimal>? Dimensions) : IRequest<Result<int>>;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        When(x => !x.ProductConfigurationId.HasValue, () =>
        {
            RuleFor(x => x.ProductId)
                .NotNull().GreaterThan(0).WithMessage("ProductId is required when configuration id is not provided.");

            RuleFor(x => x.WoodMaterialId)
                .NotNull().GreaterThan(0).WithMessage("WoodMaterialId is required.");

            RuleFor(x => x.WoodColorId)
                .NotNull().GreaterThan(0).WithMessage("WoodColorId is required.");

            RuleFor(x => x.Dimensions)
                .NotNull().WithMessage("Dimensions dictionary is required.");
        });
    }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPricingService _pricingService;

    public AddToCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPricingService pricingService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _pricingService = pricingService;
    }

    public async Task<Result<int>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<int>.Failure("User is not authenticated.");
        }

        int configurationId;

        // If configuration already exists, verify it
        if (request.ProductConfigurationId.HasValue && request.ProductConfigurationId.Value > 0)
        {
            var existingConfig = await _context.ProductConfigurations
                .Include(c => c.Dimensions)
                    .ThenInclude(cd => cd.ProductDimension)
                .FirstOrDefaultAsync(c => c.Id == request.ProductConfigurationId.Value, cancellationToken);

            if (existingConfig == null)
            {
                return Result<int>.Failure("Product configuration not found.");
            }

            var dimensionsDict = existingConfig.Dimensions
                .Where(d => d.ProductDimension != null)
                .ToDictionary(d => d.ProductDimension.Name, d => d.Value);

            var validation = await _pricingService.CalculatePriceAsync(
                existingConfig.ProductId,
                existingConfig.WoodMaterialId,
                existingConfig.WoodColorId,
                dimensionsDict,
                cancellationToken);

            if (!validation.IsSuccess)
            {
                return Result<int>.Failure(validation.Error);
            }

            configurationId = existingConfig.Id;
        }
        else
        {
            // Validate new configuration
            var validation = await _pricingService.CalculatePriceAsync(
                request.ProductId!.Value,
                request.WoodMaterialId!.Value,
                request.WoodColorId!.Value,
                request.Dimensions!,
                cancellationToken);

            if (!validation.IsSuccess)
            {
                return Result<int>.Failure(validation.Error);
            }

            var product = await _context.Products
                .Include(p => p.Dimensions)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId!.Value, cancellationToken);

            if (product == null)
            {
                return Result<int>.Failure("Product not found.");
            }

            // Create and persist the configuration
            var newConfig = new ProductConfiguration
            {
                ProductId = request.ProductId.Value,
                WoodMaterialId = request.WoodMaterialId.Value,
                WoodColorId = request.WoodColorId.Value,
                Dimensions = new List<ConfigurationDimension>()
            };

            foreach (var dim in product.Dimensions)
            {
                if (!request.Dimensions!.TryGetValue(dim.Name, out var val))
                {
                    val = dim.DefaultValue ?? dim.MinValue;
                }

                newConfig.Dimensions.Add(new ConfigurationDimension
                {
                    ProductDimensionId = dim.Id,
                    Value = val
                });
            }

            _context.ProductConfigurations.Add(newConfig);
            await _context.SaveChangesAsync(cancellationToken);

            configurationId = newConfig.Id;
        }

        // Retrieve or create User Cart
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

        if (cart == null)
        {
            cart = new Domain.Entities.Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Create and add CartItem
        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductConfigurationId = configurationId,
            AddedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(cartItem.Id);
    }
}
