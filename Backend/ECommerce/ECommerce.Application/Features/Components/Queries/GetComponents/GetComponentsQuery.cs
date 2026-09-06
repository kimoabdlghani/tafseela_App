using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Components.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Components.Queries.GetComponents;

public record GetComponentsQuery(bool OnlyActive = true) : IRequest<Result<List<ComponentDto>>>;

public class GetComponentsQueryHandler : IRequestHandler<GetComponentsQuery, Result<List<ComponentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetComponentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ComponentDto>>> Handle(GetComponentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Components.AsNoTracking().AsQueryable();

        if (request.OnlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        var components = await query
            .OrderBy(c => c.Name)
            .Select(c => new ComponentDto(
                c.Id,
                c.Name,
                c.Type,
                c.Unit,
                c.UnitPrice,
                c.IsActive,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<ComponentDto>>.Success(components);
    }
}
