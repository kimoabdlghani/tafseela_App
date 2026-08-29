using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedList<ProductListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive && !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower().Trim();
            query = query.Where(p => p.Name.ToLower().Contains(search) 
                                  || (p.Description ?? "").ToLower().Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (request.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == request.BrandId.Value);
        }

        var projectedQuery = query.Select(p => new ProductListDto(
            p.Id,
            p.Name,
            p.Category != null ? p.Category.Name : "",
            p.Brand != null ? p.Brand.Name : "",
            p.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
            p.Variants.Any(v => !v.IsDeleted) ? p.Variants.Where(v => !v.IsDeleted).Min(v => v.Price) : 0,
            p.Variants.Where(v => !v.IsDeleted).Sum(v => v.StockQuantity) > 0
        ));

        projectedQuery = request.SortBy?.ToLower() switch
        {
            "price_asc" => projectedQuery.OrderBy(p => p.StartingPrice),
            "price_desc" => projectedQuery.OrderByDescending(p => p.StartingPrice),
            "name" => projectedQuery.OrderBy(p => p.Name),
            "newest" => projectedQuery.OrderByDescending(p => p.Id),
            _ => projectedQuery.OrderByDescending(p => p.Id) // الترتيب الافتراضي
        };

        var paginatedResult = await PaginatedList<ProductListDto>.CreateAsync(
            projectedQuery, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        return Result<PaginatedList<ProductListDto>>.Success(paginatedResult);
    }
}