namespace ECommerce.Application.Features.Materials.DTOs;

public record WoodColorDto(
    int Id,
    int WoodMaterialId,
    string Name,
    bool IsActive,
    DateTime CreatedAt);

public record WoodMaterialDto(
    int Id,
    string Name,
    decimal Thickness,
    string Unit,
    decimal UnitPrice,
    bool IsActive,
    DateTime CreatedAt,
    List<WoodColorDto> Colors);
