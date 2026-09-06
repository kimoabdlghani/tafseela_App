using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.ProductionJobs.DTOs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.ProductionJobs.Queries.GetAvailableJobs;

public record GetAvailableJobsQuery : IRequest<Result<List<ProductionJobDto>>>;

public class GetAvailableJobsQueryHandler : IRequestHandler<GetAvailableJobsQuery, Result<List<ProductionJobDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAvailableJobsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ProductionJobDto>>> Handle(
        GetAvailableJobsQuery request,
        CancellationToken cancellationToken)
    {
        var jobs = await _context.ProductionJobs
            .AsNoTracking()
            .Include(j => j.Order)
            .Include(j => j.OrderItem)
            .Where(j => j.Status == ProductionJobStatus.Available)
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
                null,
                null,
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
