using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Commands.AssignJob;

public record AssignJobCommand(int JobId, int CarpenterProfileId) : IRequest<Result<bool>>;

public class AssignJobCommandValidator : AbstractValidator<AssignJobCommand>
{
    public AssignJobCommandValidator()
    {
        RuleFor(x => x.JobId).GreaterThan(0);
        RuleFor(x => x.CarpenterProfileId).GreaterThan(0);
    }
}

public class AssignJobCommandHandler : IRequestHandler<AssignJobCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AssignJobCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(AssignJobCommand request, CancellationToken cancellationToken)
    {
        var adminUserId = _currentUserService.UserId ?? 0;

        var job = await _context.ProductionJobs
            .Include(j => j.StatusHistory)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job == null)
        {
            return Result<bool>.Failure("Production job was not found.");
        }

        if (job.Status != ProductionJobStatus.Available && job.Status != ProductionJobStatus.Assigned)
        {
            return Result<bool>.Failure($"Cannot assign job currently in '{job.Status}' status.");
        }

        // Validate Carpenter Profile
        var carpenter = await _context.CarpenterProfiles
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == request.CarpenterProfileId, cancellationToken);

        if (carpenter == null)
        {
            return Result<bool>.Failure("Carpenter profile was not found.");
        }

        if (carpenter.Status != CarpenterStatus.Active)
        {
            return Result<bool>.Failure($"Cannot assign job to carpenter with status '{carpenter.Status}'. Only Active carpenters can take jobs.");
        }

        // Check MaxActiveJobs limit
        var activeJobsCount = await _context.ProductionJobs
            .CountAsync(j => j.AssignedCarpenterId == carpenter.Id &&
                             (j.Status == ProductionJobStatus.Assigned ||
                              j.Status == ProductionJobStatus.InProduction ||
                              j.Status == ProductionJobStatus.ReworkRequired),
                        cancellationToken);

        if (activeJobsCount >= carpenter.MaxActiveJobs)
        {
            return Result<bool>.Failure(
                $"Carpenter '{carpenter.User.Email}' has reached their maximum allowed active jobs capacity ({activeJobsCount}/{carpenter.MaxActiveJobs}).");
        }

        job.AssignedCarpenterId = carpenter.Id;
        job.Status = ProductionJobStatus.Assigned;
        job.AssignedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;

        job.StatusHistory.Add(new ProductionJobStatusHistory
        {
            ProductionJobId = job.Id,
            Status = ProductionJobStatus.Assigned,
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = adminUserId,
            Notes = $"Assigned to carpenter #{carpenter.Id} ({carpenter.User.Email}) by Admin.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
