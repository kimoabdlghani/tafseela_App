using System.Text.Json;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Commands.Checkout;

public record CheckoutCommand(
    int? AddressId,
    AddressSnapshotDto? Address,
    decimal DeliveryCost = 0) : IRequest<Result<int>>;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.AddressId.HasValue || x.Address != null)
            .WithMessage("Either a saved AddressId or new Address details must be provided.");

        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!.Street).NotEmpty().WithMessage("Street is required.");
            RuleFor(x => x.Address!.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.Address!.Country).NotEmpty().WithMessage("Country is required.");
        });

        RuleFor(x => x.DeliveryCost)
            .GreaterThanOrEqualTo(0).WithMessage("Delivery cost cannot be negative.");
    }
}

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPricingService _pricingService;

    public CheckoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPricingService pricingService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _pricingService = pricingService;
    }

    public async Task<Result<int>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<int>.Failure("User is not authenticated.");
        }

        // 1. Fetch User Cart
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductConfiguration)
                    .ThenInclude(pc => pc.Product)
                        .ThenInclude(p => p.ProductComponents)
                            .ThenInclude(pc => pc.Component)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductConfiguration)
                    .ThenInclude(pc => pc.WoodMaterial)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductConfiguration)
                    .ThenInclude(pc => pc.WoodColor)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductConfiguration)
                    .ThenInclude(pc => pc.Dimensions)
                        .ThenInclude(cd => cd.ProductDimension)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

        if (cart == null || !cart.Items.Any())
        {
            return Result<int>.Failure("Your cart is empty.");
        }

        // 2. Resolve Shipping Address Snapshot
        AddressSnapshotDto addressSnapshot;
        if (request.Address != null)
        {
            addressSnapshot = request.Address;
        }
        else
        {
            var savedAddress = await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == request.AddressId!.Value && a.UserId == userId.Value, cancellationToken);

            if (savedAddress == null)
            {
                return Result<int>.Failure("Selected saved address was not found.");
            }

            addressSnapshot = new AddressSnapshotDto(
                Street: savedAddress.Street,
                City: savedAddress.City,
                Building: savedAddress.Building,
                PostalCode: savedAddress.PostalCode,
                Country: savedAddress.Country,
                Apartment: savedAddress.Apartment,
                Floor: savedAddress.Floor,
                Notes: null);
        }

        // 3. Re-validate each cart item and calculate frozen snapshot values
        var verifiedItems = new List<(CartItem Item, PriceBreakdown Price, string ComponentsJson)>();

        foreach (var cartItem in cart.Items)
        {
            var cfg = cartItem.ProductConfiguration;
            if (cfg == null || cfg.Product == null)
            {
                return Result<int>.Failure($"Cart item #{cartItem.Id} has missing product configuration.");
            }

            var dimensionsDict = cfg.Dimensions
                .Where(d => d.ProductDimension != null)
                .ToDictionary(d => d.ProductDimension.Name, d => d.Value);

            var priceResult = await _pricingService.CalculatePriceAsync(
                cfg.ProductId,
                cfg.WoodMaterialId,
                cfg.WoodColorId,
                dimensionsDict,
                cancellationToken);

            if (!priceResult.IsSuccess)
            {
                return Result<int>.Failure(
                    $"Cart item for '{cfg.Product.Name}' is no longer valid: {priceResult.Error}. Please update your cart.");
            }

            // Capture component snapshot list
            var componentsList = cfg.Product.ProductComponents
                .Where(c => c.Component != null)
                .Select(c => new
                {
                    Name = c.Component.Name,
                    Quantity = c.Quantity,
                    UnitPrice = c.Component.UnitPrice,
                    Total = c.Quantity * c.Component.UnitPrice
                }).ToList();

            var componentsJson = JsonSerializer.Serialize(componentsList);

            verifiedItems.Add((cartItem, priceResult.Data!, componentsJson));
        }

        // 4. Fetch Active Company Settings
        var settings = await _context.CompanySettings
            .AsNoTracking()
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        var depositPercentage = settings?.DepositPercentage ?? 0.30m;

        // 5. Calculate Order Totals
        var productTotal = verifiedItems.Sum(v => v.Price.SellingPrice);
        var deliveryCost = Math.Max(0m, request.DeliveryCost);
        var orderTotal = productTotal + deliveryCost;
        var depositAmount = Math.Round(orderTotal * depositPercentage, 2);

        // 6. Create Order
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = userId.Value,
            Status = OrderStatus.Pending,
            ProductTotal = productTotal,
            DeliveryCost = deliveryCost,
            OrderTotal = orderTotal,
            DepositAmount = depositAmount,
            DepositPercentageSnapshot = depositPercentage,
            AddressSnapshot = JsonSerializer.Serialize(addressSnapshot),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };

        foreach (var (cartItem, price, compJson) in verifiedItems)
        {
            var orderItem = new OrderItem
            {
                ProductConfigurationId = cartItem.ProductConfigurationId,
                ProductNameSnapshot = price.ProductName,
                ProductDescriptionSnapshot = cartItem.ProductConfiguration.Product.Description ?? string.Empty,
                WoodMaterialNameSnapshot = price.WoodMaterialName,
                WoodColorNameSnapshot = price.WoodColorName,
                WoodUnitPriceSnapshot = price.WoodUnitPrice,
                DimensionsSnapshot = JsonSerializer.Serialize(price.DimensionsSnapshot),
                ComponentsSnapshot = compJson,
                MaterialCostSnapshot = price.MaterialCost,
                CarpenterAmountSnapshot = price.CarpenterAmount,
                CompanyProfitSnapshot = price.CompanyProfit,
                SellingPrice = price.SellingPrice,
                CarpenterPercentageSnapshot = price.CarpenterPercentage,
                CompanyProfitPercentageSnapshot = price.CompanyProfitPercentage,
                Quantity = 1,
                TotalPrice = price.SellingPrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            order.OrderItems.Add(orderItem);
        }

        _context.Orders.Add(order);

        // 7. Clear checked out items from Cart
        _context.CartItems.RemoveRange(verifiedItems.Select(v => v.Item));

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(order.Id);
    }
}
