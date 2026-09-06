using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Commands.StartJob;

public record StartJobCommand(int JobId) : IRequest<Result<bool>>;

public class StartJobCommandHandler : IRequestHandler<StartJobCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public StartJobCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(StartJobCommand request, CancellationToken cancellationToken)
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
            .Include(j => j.Order)
            .FirstOrDefaultAsync(j => j.Id == request.JobId && j.AssignedCarpenterId == carpenter.Id, cancellationToken);

        if (job == null)
        {
            return Result<bool>.Failure("Job not found or not assigned to you.");
        }

        if (job.Status != ProductionJobStatus.Assigned && job.Status != ProductionJobStatus.ReworkRequired)
        {
            return Result<bool>.Failure($"Cannot start job in '{job.Status}' status.");
        }

        job.Status = ProductionJobStatus.InProduction;
        job.StartedAt ??= DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;

        job.StatusHistory.Add(new ProductionJobStatusHistory
        {
            ProductionJobId = job.Id,
            Status = ProductionJobStatus.InProduction,
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = userId.Value,
            Notes = "Carpenter started work on this job.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Also update Order status to InProduction if it's still DepositPaid
        if (job.Order.Status == OrderStatus.DepositPaid)
        {
            job.Order.Status = OrderStatus.InProduction;
            job.Order.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
