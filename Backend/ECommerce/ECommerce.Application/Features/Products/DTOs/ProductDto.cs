using ECommerce.Application.Common.Mappings;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Products.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    ProductStatus Status,
    bool IsActive,
    int CategoryId,
    string CategoryName) : IMapFrom<Product>;