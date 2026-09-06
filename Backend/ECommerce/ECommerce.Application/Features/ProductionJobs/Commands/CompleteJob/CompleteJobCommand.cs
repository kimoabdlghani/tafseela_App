using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Commands.CompleteJob;

public record CompleteJobCommand(int JobId) : IRequest<Result<bool>>;

public class CompleteJobCommandHandler : IRequestHandler<CompleteJobCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CompleteJobCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(CompleteJobCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<bool>.Failure("User is not authenticated.");
        }

        var carpenter = await _context.CarpenterProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

        if (carpenter == null)
        {
            return Result<bool>.Failure("Carpenter profile not found.");
        }

        var job = await _context.ProductionJobs
            .Include(j => j.StatusHistory)
            .FirstOrDefaultAsync(j => j.Id == request.JobId && j.AssignedCarpenterId == carpenter.Id, cancellationToken);

        if (job == null)
        {
            return Result<bool>.Failure("Job not found or not assigned to you.");
        }

        if (job.Status != ProductionJobStatus.InProduction)
        {
            return Result<bool>.Failure($"Cannot complete job with status '{job.Status}'. Must be 'InProduction'.");
        }

        job.Status = ProductionJobStatus.CarpenterCompleted;
        job.CarpenterCompletedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;

        job.StatusHistory.Add(new ProductionJobStatusHistory
        {
            ProductionJobId = job.Id,
            Status = ProductionJobStatus.CarpenterCompleted,
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = userId.Value,
            Notes = "Carpenter finished manufacturing. Awaiting Admin QC inspection.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
