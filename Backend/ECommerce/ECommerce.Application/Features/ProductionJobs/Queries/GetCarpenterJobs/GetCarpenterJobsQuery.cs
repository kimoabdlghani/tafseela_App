using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.ProductionJobs.DTOs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Queries.GetCarpenterJobs;

public record GetCarpenterJobsQuery(ProductionJobStatus? Status = null) : IRequest<Result<List<ProductionJobDto>>>;

public class GetCarpenterJobsQueryHandler : IRequestHandler<GetCarpenterJobsQuery, Result<List<ProductionJobDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCarpenterJobsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<ProductionJobDto>>> Handle(
        GetCarpenterJobsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<List<ProductionJobDto>>.Failure("User is not authenticated.");
        }

        var carpenter = await _context.CarpenterProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

        if (carpenter == null)
        {
            return Result<List<ProductionJobDto>>.Failure("Carpenter profile was not found.");
        }

        var query = _context.ProductionJobs
            .AsNoTracking()
            .Include(j => j.Order)
            .Include(j => j.OrderItem)
            .Where(j => j.AssignedCarpenterId == carpenter.Id);

        if (request.Status.HasValue)
        {
            query = query.Where(j => j.Status == request.Status.Value);
        }

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new ProductionJobDto(
                j.Id,
                j.OrderId,
                j.Order.OrderNumber,
                j.OrderItemId,
                j.OrderItem.ProductNameSnapshot,
                j.OrderItem.WoodMaterialNameSnapshot,
                j.OrderItem.WoodColorNameSnapshot,
                j.Status.ToString(),
                j.OrderItem.CarpenterAmountSnapshot,
                j.ProductionSnapshot,
                j.AssignedCarpenterId,
                _currentUserService.Email,
                j.AssignedAt,
                j.StartedAt,
                j.CarpenterCompletedAt,
                j.AdminInspectedAt,
                j.ReworkNotes,
                j.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<ProductionJobDto>>.Success(jobs);
    }
}
