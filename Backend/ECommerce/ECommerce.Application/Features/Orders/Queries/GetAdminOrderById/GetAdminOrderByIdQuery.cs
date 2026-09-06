using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Queries.GetAdminOrderById;

public record GetAdminOrderByIdQuery(int OrderId) : IRequest<Result<OrderDetailDto>>;

public class GetAdminOrderByIdQueryHandler : IRequestHandler<GetAdminOrderByIdQuery, Result<OrderDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderDetailDto>> Handle(
        GetAdminOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductionJob)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result<OrderDetailDto>.Failure("Order was not found.");
        }

        var itemDtos = order.OrderItems.Select(oi => new OrderItemDto(
            Id: oi.Id,
            ProductConfigurationId: oi.ProductConfigurationId,
            ProductName: oi.ProductNameSnapshot,
            WoodMaterialName: oi.WoodMaterialNameSnapshot,
            WoodColorName: oi.WoodColorNameSnapshot,
            DimensionsSnapshot: oi.DimensionsSnapshot,
            ComponentsSnapshot: oi.ComponentsSnapshot,
            MaterialCost: oi.MaterialCostSnapshot,
            CarpenterAmount: oi.CarpenterAmountSnapshot,
            CompanyProfit: oi.CompanyProfitSnapshot,
            SellingPrice: oi.SellingPrice,
            Quantity: oi.Quantity,
            TotalPrice: oi.TotalPrice,
            ProductionJobStatus: oi.ProductionJob?.Status.ToString(),
            ProductionJobId: oi.ProductionJob?.Id
        )).ToList();

        var dto = new OrderDetailDto(
            Id: order.Id,
            OrderNumber: order.OrderNumber,
            UserId: order.UserId,
            CustomerEmail: order.User.Email ?? string.Empty,
            Status: order.Status.ToString(),
            ProductTotal: order.ProductTotal,
            DeliveryCost: order.DeliveryCost,
            OrderTotal: order.OrderTotal,
            DepositAmount: order.DepositAmount,
            DepositPercentageSnapshot: order.DepositPercentageSnapshot,
            AddressSnapshot: order.AddressSnapshot,
            CreatedAt: order.CreatedAt,
            Items: itemDtos);

        return Result<OrderDetailDto>.Success(dto);
    }
}
