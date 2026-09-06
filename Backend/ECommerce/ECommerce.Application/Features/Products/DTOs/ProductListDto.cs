namespace ECommerce.Application.Features.Products.DTOs;

public record ProductListDto(
    int Id,
    string Name,
    string CategoryName,
    string? PrimaryImageUrl,
    bool IsActive
);