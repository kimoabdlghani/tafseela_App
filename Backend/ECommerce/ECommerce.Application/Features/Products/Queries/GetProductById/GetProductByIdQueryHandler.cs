using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Materials.DTOs;
using ECommerce.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;

    public GetProductByIdQueryHandler(
        IApplicationDbContext context,
        IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Dimensions)
            .Include(p => p.ProductWoodMaterials)
                .ThenInclude(pwm => pwm.WoodMaterial)
                    .ThenInclude(wm => wm.Colors)
            .Include(p => p.ProductComponents)
                .ThenInclude(pc => pc.Component)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (product == null)
        {
            return Result<ProductDetailDto>.Failure($"Product with Id {request.Id} was not found.");
        }

        var imageDtos = product.Images
            .OrderByDescending(i => i.IsPrimary)
            .Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.IsPrimary))
            .ToList();

        var dimensionDtos = product.Dimensions
            .OrderBy(d => d.DisplayOrder)
            .Select(d => new ProductDimensionDto(d.Id, d.Name, d.Unit, d.MinValue, d.MaxValue, d.DefaultValue, d.DisplayOrder))
            .ToList();

        var woodOptionDtos = product.ProductWoodMaterials
            .Where(pwm => pwm.WoodMaterial != null && pwm.WoodMaterial.IsActive)
            .Select(pwm => new ProductWoodMaterialOptionDto(
                pwm.WoodMaterial.Id,
                pwm.WoodMaterial.Name,
                pwm.WoodMaterial.Thickness,
                pwm.WoodMaterial.Unit,
                pwm.WoodMaterial.UnitPrice,
                pwm.WoodMaterial.Colors
                    .Where(c => c.IsActive)
                    .Select(c => new WoodColorDto(c.Id, c.WoodMaterialId, c.Name, c.IsActive, c.CreatedAt))
                    .ToList()))
            .ToList();

        var componentDtos = product.ProductComponents
            .Where(pc => pc.Component != null && pc.Component.IsActive)
            .Select(pc => new ProductComponentOptionDto(
                pc.Component.Id,
                pc.Component.Name,
                pc.Component.Type.ToString(),
                pc.Quantity))
            .ToList();

        // Calculate baseline/starting price with default options
        decimal startingPrice = 0m;
        var defaultWood = woodOptionDtos.FirstOrDefault();
        var defaultColor = defaultWood?.Colors.FirstOrDefault();

        if (defaultWood != null && defaultColor != null && dimensionDtos.Any())
        {
            var defaultDims = dimensionDtos.ToDictionary(
                d => d.Name,
                d => d.DefaultValue ?? d.MinValue);

            var pricePreview = await _pricingService.CalculatePriceAsync(
                product.Id,
                defaultWood.Id,
                defaultColor.Id,
                defaultDims,
                cancellationToken);

            if (pricePreview.IsSuccess)
            {
                startingPrice = pricePreview.Data.SellingPrice;
            }
        }

        var dto = new ProductDetailDto(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description ?? string.Empty,
            Status: product.Status,
            IsActive: product.IsActive,
            CategoryId: product.CategoryId,
            CategoryName: product.Category?.Name ?? string.Empty,
            Images: imageDtos,
            Dimensions: dimensionDtos,
            AllowedWoodMaterials: woodOptionDtos,
            Components: componentDtos,
            EstimatedStartingPrice: startingPrice);

        return Result<ProductDetailDto>.Success(dto);
    }
}