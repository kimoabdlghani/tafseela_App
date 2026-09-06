using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Carpenters.DTOs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carpenters.Queries.GetProfiles;

public record GetCarpenterProfilesQuery(CarpenterStatus? Status = null) : IRequest<Result<List<CarpenterProfileDto>>>;

public class GetCarpenterProfilesQueryHandler : IRequestHandler<GetCarpenterProfilesQuery, Result<List<CarpenterProfileDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCarpenterProfilesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CarpenterProfileDto>>> Handle(
        GetCarpenterProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.CarpenterProfiles
            .AsNoTracking()
            .Include(cp => cp.User)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(cp => cp.Status == request.Status.Value);
        }

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

        var profiles = await query
            .OrderByDescending(cp => cp.JoinedAt)
            .Select(cp => new CarpenterProfileDto(
                cp.Id,
                cp.UserId,
                cp.User.Email ?? string.Empty,
                cp.MaxActiveJobs,
                cp.Status.ToString(),
                cp.JoinedAt,
                _context.ProductionJobs.Count(j => j.AssignedCarpenterId == cp.Id && activeStatuses.Contains(j.Status)),
                _context.ProductionJobs.Count(j => j.AssignedCarpenterId == cp.Id && completedStatuses.Contains(j.Status))
            ))
            .ToListAsync(cancellationToken);

        return Result<List<CarpenterProfileDto>>.Success(profiles);
    }
}
