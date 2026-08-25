using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<IEnumerable<OrderListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetOrdersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<OrderListDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Result<IEnumerable<OrderListDto>>.Failure("Unauthorized");

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListDto(
                o.Id,
                o.CreatedAt,
                o.TotalAmount,
                o.OrderStatus.ToString() 
            ))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<OrderListDto>>.Success(orders);
    }
}