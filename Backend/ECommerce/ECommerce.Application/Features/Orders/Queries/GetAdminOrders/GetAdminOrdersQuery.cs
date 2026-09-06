using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.DTOs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetAdminOrders;

public record GetAdminOrdersQuery(
    OrderStatus? Status = null,
    string? SearchTerm = null) : IRequest<Result<List<OrderListDto>>>;

public class GetAdminOrdersQueryHandler : IRequestHandler<GetAdminOrdersQuery, Result<List<OrderListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<OrderListDto>>> Handle(
        GetAdminOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(o => o.OrderNumber.ToLower().Contains(term) ||
                                     (o.User != null && o.User.Email != null && o.User.Email.ToLower().Contains(term)));
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListDto(
                o.Id,
                o.OrderNumber,
                o.User.Email ?? string.Empty,
                o.Status.ToString(),
                o.ProductTotal,
                o.DeliveryCost,
                o.OrderTotal,
                o.DepositAmount,
                o.OrderItems.Count,
                o.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<OrderListDto>>.Success(orders);
    }
}
