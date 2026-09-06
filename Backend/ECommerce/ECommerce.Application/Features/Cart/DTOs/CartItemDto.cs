namespace ECommerce.Application.Features.Cart.DTOs;

public record CartItemDto(
    int Id,
    int ProductConfigurationId,
    int ProductId,
    string ProductName,
    string WoodMaterialName,
    string WoodColorName,
    Dictionary<string, decimal> Dimensions,
    decimal SellingPrice,
    decimal DepositAmount,
    string? PrimaryImageUrl,
    bool IsValid,
    string? ValidationErrorMessage,
    DateTime AddedAt);