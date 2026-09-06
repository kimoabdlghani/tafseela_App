using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Components.DTOs;

public record ComponentDto(
    int Id,
    string Name,
    ComponentType Type,
    string Unit,
    decimal UnitPrice,
    bool IsActive,
    DateTime CreatedAt);
