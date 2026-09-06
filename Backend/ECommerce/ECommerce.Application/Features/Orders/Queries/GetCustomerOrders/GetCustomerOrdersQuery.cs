using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetCustomerOrders;

public record GetCustomerOrdersQuery : IRequest<Result<List<OrderListDto>>>;

public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, Result<List<OrderListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCustomerOrdersQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<OrderListDto>>> Handle(
        GetCustomerOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<List<OrderListDto>>.Failure("User is not authenticated.");
        }

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId.Value)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListDto(
                o.Id,
                o.OrderNumber,
                _currentUserService.Email,
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
