namespace ECommerce.Application.Features.Carpenters.DTOs;

public record CarpenterApplicationDto(
    int Id,
    int UserId,
    string UserEmail,
    string Status,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    string? ReviewNotes);

public record CarpenterProfileDto(
    int Id,
    int UserId,
    string UserEmail,
    int MaxActiveJobs,
    string Status,
    DateTime JoinedAt,
    int ActiveJobsCount,
    int CompletedJobsCount);
