using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Carpenters.DTOs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carpenters.Queries.GetApplications;

public record GetCarpenterApplicationsQuery(CarpenterApplicationStatus? Status = null) : IRequest<Result<List<CarpenterApplicationDto>>>;

public class GetCarpenterApplicationsQueryHandler : IRequestHandler<GetCarpenterApplicationsQuery, Result<List<CarpenterApplicationDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCarpenterApplicationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CarpenterApplicationDto>>> Handle(
        GetCarpenterApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.CarpenterApplications
            .AsNoTracking()
            .Include(a => a.User)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        var apps = await query
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => new CarpenterApplicationDto(
                a.Id,
                a.UserId,
                a.User.Email ?? string.Empty,
                a.Status.ToString(),
                a.SubmittedAt,
                a.ReviewedAt,
                a.ReviewNotes))
            .ToListAsync(cancellationToken);

        return Result<List<CarpenterApplicationDto>>.Success(apps);
    }
}
