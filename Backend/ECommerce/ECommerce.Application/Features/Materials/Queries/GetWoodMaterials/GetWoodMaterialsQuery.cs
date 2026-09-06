using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Materials.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Materials.Queries.GetWoodMaterials;

public record GetWoodMaterialsQuery(bool OnlyActive = true) : IRequest<Result<List<WoodMaterialDto>>>;

public class GetWoodMaterialsQueryHandler : IRequestHandler<GetWoodMaterialsQuery, Result<List<WoodMaterialDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetWoodMaterialsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WoodMaterialDto>>> Handle(
        GetWoodMaterialsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.WoodMaterials
            .AsNoTracking()
            .Include(wm => wm.Colors)
            .AsQueryable();

        if (request.OnlyActive)
        {
            query = query.Where(wm => wm.IsActive);
        }

        var materials = await query
            .OrderBy(wm => wm.Name)
            .Select(wm => new WoodMaterialDto(
                wm.Id,
                wm.Name,
                wm.Thickness,
                wm.Unit,
                wm.UnitPrice,
                wm.IsActive,
                wm.CreatedAt,
                wm.Colors
                    .Where(c => !request.OnlyActive || c.IsActive)
                    .Select(c => new WoodColorDto(c.Id, c.WoodMaterialId, c.Name, c.IsActive, c.CreatedAt))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return Result<List<WoodMaterialDto>>.Success(materials);
    }
}
