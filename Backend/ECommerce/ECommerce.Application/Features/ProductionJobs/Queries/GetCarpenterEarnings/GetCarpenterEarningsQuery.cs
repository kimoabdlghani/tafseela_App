using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Queries.GetCarpenterEarnings;

public record CarpenterEarningsDto(
    int CarpenterProfileId,
    int MaxActiveJobs,
    int CurrentActiveJobs,
    int CompletedJobsCount,
    decimal TotalEarned,
    decimal PendingEarnings);

public record GetCarpenterEarningsQuery : IRequest<Result<CarpenterEarningsDto>>;

public class GetCarpenterEarningsQueryHandler : IRequestHandler<GetCarpenterEarningsQuery, Result<CarpenterEarningsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCarpenterEarningsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CarpenterEarningsDto>> Handle(
        GetCarpenterEarningsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<CarpenterEarningsDto>.Failure("User is not authenticated.");
        }

        var carpenter = await _context.CarpenterProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

        if (carpenter == null)
        {
            return Result<CarpenterEarningsDto>.Failure("Carpenter profile was not found.");
        }

        var jobs = await _context.ProductionJobs
            .AsNoTracking()
            .Include(j => j.OrderItem)
            .Where(j => j.AssignedCarpenterId == carpenter.Id)
            .ToListAsync(cancellationToken);

        var activeStatuses = new[]
        {
            ProductionJobStatus.Assigned,
            ProductionJobStatus.InProduction,
            ProductionJobStatus.ReworkRequired
        };

        var completedStatuses = new[]
        {
            ProductionJobStatus.Ready,
            ProductionJobStatus.Shipping,
            ProductionJobStatus.Delivered
        };

        var pendingStatuses = new[]
        {
            ProductionJobStatus.Assigned,
            ProductionJobStatus.InProduction,
            ProductionJobStatus.CarpenterCompleted,
            ProductionJobStatus.ReworkRequired
        };

        var currentActiveCount = jobs.Count(j => activeStatuses.Contains(j.Status));
        var completedCount = jobs.Count(j => completedStatuses.Contains(j.Status));
        var totalEarned = jobs.Where(j => completedStatuses.Contains(j.Status)).Sum(j => j.OrderItem.CarpenterAmountSnapshot);
        var pendingEarnings = jobs.Where(j => pendingStatuses.Contains(j.Status)).Sum(j => j.OrderItem.CarpenterAmountSnapshot);

        var dto = new CarpenterEarningsDto(
            CarpenterProfileId: carpenter.Id,
            MaxActiveJobs: carpenter.MaxActiveJobs,
            CurrentActiveJobs: currentActiveCount,
            CompletedJobsCount: completedCount,
            TotalEarned: totalEarned,
            PendingEarnings: pendingEarnings);

        return Result<CarpenterEarningsDto>.Success(dto);
    }
}
