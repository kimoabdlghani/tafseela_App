using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<OrderDetailDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Result<OrderDetailDto>.Failure("Unauthorized");

        var orderDetail = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == request.Id && o.UserId == userId)
            .Select(o => new OrderDetailDto(
                o.Id,
                o.CreatedAt,
                o.TotalAmount,
                o.OrderStatus.ToString(),
                o.Address != null 
            ? $"{o.Address.Building}, {o.Address.Street}, {o.Address.City}, {o.Address.Country}" 
            : "No Address Provided",
                o.Payment != null ? o.Payment.PaymentMethod : "Pending Payment",
                o.OrderItems.Select(oi => new OrderItemDto(
                    oi.Id,
                    oi.ProductVariantId,
                    oi.ProductVariant.Product.Name,
                    oi.ProductVariant.Color,
                    oi.ProductVariant.Size,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.ProductVariant.Product.Images.Where(img => img.IsPrimary).Select(img => img.ImageUrl).FirstOrDefault()
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (orderDetail == null)
        {
            return Result<OrderDetailDto>.Failure("Order not found or you do not have permission to view it.");
        }

        return Result<OrderDetailDto>.Success(orderDetail);
    }
}