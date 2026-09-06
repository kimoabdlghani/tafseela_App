namespace ECommerce.Application.Features.ProductionJobs.DTOs;

public record JobStatusHistoryDto(
    int Id,
    string Status,
    DateTime ChangedAt,
    string ChangedByUserName,
    string? Notes);

public record ProductionJobDto(
    int Id,
    int OrderId,
    string OrderNumber,
    int OrderItemId,
    string ProductName,
    string WoodMaterial,
    string WoodColor,
    string Status,
    decimal CarpenterPayout,
    string ProductionSnapshot,
    int? AssignedCarpenterId,
    string? AssignedCarpenterName,
    DateTime? AssignedAt,
    DateTime? StartedAt,
    DateTime? CarpenterCompletedAt,
    DateTime? AdminInspectedAt,
    string? ReworkNotes,
    DateTime CreatedAt);

public record ProductionJobDetailDto(
    int Id,
    int OrderId,
    string OrderNumber,
    int OrderItemId,
    string ProductName,
    string WoodMaterial,
    string WoodColor,
    string Status,
    decimal CarpenterPayout,
    string ProductionSnapshot,
    int? AssignedCarpenterId,
    string? AssignedCarpenterName,
    DateTime? AssignedAt,
    DateTime? StartedAt,
    DateTime? CarpenterCompletedAt,
    DateTime? AdminInspectedAt,
    string? ReworkNotes,
    DateTime CreatedAt,
    List<JobStatusHistoryDto> StatusHistory);
