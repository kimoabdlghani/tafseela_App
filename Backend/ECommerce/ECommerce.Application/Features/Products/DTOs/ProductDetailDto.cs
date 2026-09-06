using ECommerce.Application.Features.Materials.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Products.DTOs;

public record ProductDimensionDto(
    int Id,
    string Name,
    string Unit,
    decimal MinValue,
    decimal MaxValue,
    decimal? DefaultValue,
    int DisplayOrder);

public record ProductWoodMaterialOptionDto(
    int Id,
    string Name,
    decimal Thickness,
    string Unit,
    decimal UnitPrice,
    List<WoodColorDto> Colors);

public record ProductComponentOptionDto(
    int Id,
    string Name,
    string Type,
    int Quantity);

public record ProductDetailDto(
    int Id,
    string Name,
    string Description,
    ProductStatus Status,
    bool IsActive,
    int CategoryId,
    string CategoryName,
    List<ProductImageDto> Images,
    List<ProductDimensionDto> Dimensions,
    List<ProductWoodMaterialOptionDto> AllowedWoodMaterials,
    List<ProductComponentOptionDto> Components,
    decimal EstimatedStartingPrice);