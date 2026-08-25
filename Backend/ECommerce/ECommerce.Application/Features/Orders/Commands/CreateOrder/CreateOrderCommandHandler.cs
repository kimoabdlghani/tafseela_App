using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Result<int>.Failure("Unauthorized");

        var cartItems = await _context.CartItems
            .Include(c => c.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);

        if (!cartItems.Any())
            return Result<int>.Failure("Your cart is empty.");

        foreach (var item in cartItems)
        {
            if (item.ProductVariant.IsDeleted)
                return Result<int>.Failure($"One of the items in your cart is no longer available.");

            if (item.Quantity > item.ProductVariant.StockQuantity)
                return Result<int>.Failure($"Insufficient stock for variant ID {item.ProductVariant.Id}. Available: {item.ProductVariant.StockQuantity}.");
        }

        var addressExists = await _context.Addresses
            .AnyAsync(a => a.Id == request.AddressId && a.UserId == userId && !a.IsDeleted, cancellationToken);
            
        if (!addressExists)
            return Result<int>.Failure("Invalid shipping address selected.");

        decimal totalPrice = cartItems.Sum(c => c.Quantity * c.ProductVariant.Price);

        var order = new Order
        {
            UserId = userId.Value, 
            AddressId = request.AddressId,
            TotalAmount = totalPrice,
            OrderStatus = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in cartItems)
        {
            var orderItem = new OrderItem
            {
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                UnitPrice = item.ProductVariant.Price ,
                ProductName= item.ProductVariant.Product.Name,
                Color=item.ProductVariant.Color,
                Size=item.ProductVariant.Size,
                TotalPrice=item.Quantity*item.ProductVariant.Price
            };
            
            order.OrderItems.Add(orderItem);
            item.ProductVariant.StockQuantity -= item.Quantity;
        }

        var payment = new Payment
        {
            Amount = totalPrice,
            PaymentMethod = request.PaymentMethod ?? string.Empty,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TransactionId = string.Empty
        };
        
        order.Payment = payment;
        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(order.Id);
    }
}