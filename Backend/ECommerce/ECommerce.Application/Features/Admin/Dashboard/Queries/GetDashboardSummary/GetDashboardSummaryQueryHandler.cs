using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Admin.Dashboard.DTOs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Admin.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, Result<DashboardSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardSummaryDto>> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = await _context.Users
            .CountAsync(u => u.UserStatus == UserStatus.Active, cancellationToken);

        var totalProducts = await _context.Products
            .CountAsync(p => !p.IsDeleted, cancellationToken);

        var totalOrders = await _context.Orders.CountAsync(cancellationToken);

        var pendingOrdersCount = await _context.Orders
            .CountAsync(o => o.OrderStatus == OrderStatus.Pending, cancellationToken);

        var totalRevenue = await _context.Orders
            .Where(o => o.OrderStatus != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount, cancellationToken);

        var recentOrders = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new RecentOrderDto(
                o.Id,
                $"{o.User.FirstName} {o.User.LastName}", 
                o.TotalAmount,
                o.OrderStatus.ToString(),
                o.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var summary = new DashboardSummaryDto(
            totalUsers,
            totalProducts,
            totalOrders,
            totalRevenue,
            pendingOrdersCount,
            recentOrders
        );

        return Result<DashboardSummaryDto>.Success(summary);
    }
}