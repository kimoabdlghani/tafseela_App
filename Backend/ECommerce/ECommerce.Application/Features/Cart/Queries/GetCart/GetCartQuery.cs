using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Cart.DTOs;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery : IRequest<Result<CartDto>>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPricingService _pricingService;

    public GetCartQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPricingService pricingService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _pricingService = pricingService;
    }

    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<CartDto>.Failure("User is not authenticated.");
        }

        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductConfiguration)
                    .ThenInclude(pc => pc.Product)
                        .ThenInclude(p => p.Images)
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

        if (cart == null)
        {
            return Result<CartDto>.Success(new CartDto(new List<CartItemDto>()));
        }

        var itemDtos = new List<CartItemDto>();

        foreach (var item in cart.Items.OrderByDescending(i => i.AddedAt))
        {
            var config = item.ProductConfiguration;
            if (config == null || config.Product == null)
            {
                continue;
            }

            var dimensionsDict = config.Dimensions
                .Where(d => d.ProductDimension != null)
                .ToDictionary(d => d.ProductDimension.Name, d => d.Value);

            // Re-evaluate price and validity against current master catalog
            var priceResult = await _pricingService.CalculatePriceAsync(
                config.ProductId,
                config.WoodMaterialId,
                config.WoodColorId,
                dimensionsDict,
                cancellationToken);

            var primaryImg = config.Product.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                ?? config.Product.Images?.FirstOrDefault()?.ImageUrl;

            if (priceResult.IsSuccess)
            {
                var p = priceResult.Data;
                itemDtos.Add(new CartItemDto(
                    Id: item.Id,
                    ProductConfigurationId: config.Id,
                    ProductId: config.ProductId,
                    ProductName: config.Product.Name,
                    WoodMaterialName: config.WoodMaterial?.Name ?? p.WoodMaterialName,
                    WoodColorName: config.WoodColor?.Name ?? p.WoodColorName,
                    Dimensions: dimensionsDict,
                    SellingPrice: p.SellingPrice,
                    DepositAmount: p.DepositAmount,
                    PrimaryImageUrl: primaryImg,
                    IsValid: true,
                    ValidationErrorMessage: null,
                    AddedAt: item.AddedAt));
            }
            else
            {
                itemDtos.Add(new CartItemDto(
                    Id: item.Id,
                    ProductConfigurationId: config.Id,
                    ProductId: config.ProductId,
                    ProductName: config.Product.Name,
                    WoodMaterialName: config.WoodMaterial?.Name ?? "Unknown",
                    WoodColorName: config.WoodColor?.Name ?? "Unknown",
                    Dimensions: dimensionsDict,
                    SellingPrice: 0m,
                    DepositAmount: 0m,
                    PrimaryImageUrl: primaryImg,
                    IsValid: false,
                    ValidationErrorMessage: priceResult.Error,
                    AddedAt: item.AddedAt));
            }
        }

        return Result<CartDto>.Success(new CartDto(itemDtos));
    }
}
