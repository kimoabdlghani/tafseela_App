using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Commands.InspectJob;

public record InspectJobCommand(
    int JobId,
    bool IsApproved,
    string? ReworkNotes = null) : IRequest<Result<bool>>;

public class InspectJobCommandValidator : AbstractValidator<InspectJobCommand>
{
    public InspectJobCommandValidator()
    {
        RuleFor(x => x.JobId).GreaterThan(0);
        When(x => !x.IsApproved, () =>
        {
            RuleFor(x => x.ReworkNotes)
                .NotEmpty()
                .WithMessage("Rework notes explaining the defect/fix are required when rejecting inspection.");
        });
    }
}

public class InspectJobCommandHandler : IRequestHandler<InspectJobCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public InspectJobCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(InspectJobCommand request, CancellationToken cancellationToken)
    {
        var adminUserId = _currentUserService.UserId ?? 0;

        var job = await _context.ProductionJobs
            .Include(j => j.StatusHistory)
            .Include(j => j.Order)
                .ThenInclude(o => o.ProductionJobs)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job == null)
        {
            return Result<bool>.Failure("Production job was not found.");
        }

        if (job.Status != ProductionJobStatus.CarpenterCompleted)
        {
            return Result<bool>.Failure($"Cannot inspect job with status '{job.Status}'. Must be 'CarpenterCompleted'.");
        }

        job.AdminInspectedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;

        if (request.IsApproved)
        {
            job.Status = ProductionJobStatus.Ready;
            job.ReadyAt = DateTime.UtcNow;
            job.ReworkNotes = null;

            job.StatusHistory.Add(new ProductionJobStatusHistory
            {
                ProductionJobId = job.Id,
                Status = ProductionJobStatus.Ready,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = adminUserId,
                Notes = "Admin passed quality inspection. Job is Ready.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // If ALL jobs for this Order are now Ready (or Delivered), update Order status to Ready
            var orderJobs = job.Order.ProductionJobs;
            var allReady = orderJobs.All(oj => oj.Id == job.Id || oj.Status == ProductionJobStatus.Ready || oj.Status == ProductionJobStatus.Delivered);
            if (allReady)
            {
                job.Order.Status = OrderStatus.Ready;
                job.Order.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            job.Status = ProductionJobStatus.ReworkRequired;
            job.ReworkNotes = request.ReworkNotes;

            job.StatusHistory.Add(new ProductionJobStatusHistory
            {
                ProductionJobId = job.Id,
                Status = ProductionJobStatus.ReworkRequired,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = adminUserId,
                Notes = $"Quality inspection rejected: {request.ReworkNotes}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
