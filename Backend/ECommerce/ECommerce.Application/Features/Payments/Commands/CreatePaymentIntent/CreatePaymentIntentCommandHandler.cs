using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent;

public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPaymentService _paymentService;

    public CreatePaymentIntentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPaymentService paymentService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _paymentService = paymentService;
    }

    public async Task<Result<string>> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Result<string>.Failure("Unauthorized.");

        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken);

        if (order == null)
            return Result<string>.Failure("Order not found.");

        if (order.OrderStatus.ToString() == "Cancelled")
            return Result<string>.Failure("Cannot pay for a cancelled order.");
            
        if (order.OrderStatus != OrderStatus.Pending)
            return Result<string>.Failure("Payment can only be initiated for orders with Pending status.");

        if (order.Payment == null)
            return Result<string>.Failure("No payment record found for this order.");

        if (order.Payment.PaymentStatus == PaymentStatus.Paid)
            return Result<string>.Failure("This order has already been paid.");

        var transactionId = await _paymentService.CreatePaymentTransactionAsync(
            order.Id,
            order.TotalAmount,
            _currentUserService.Email,
            cancellationToken: cancellationToken);

        if (string.IsNullOrEmpty(transactionId))
        {
            return Result<string>.Failure("Failed to initialize payment gateway.");
        }
        order.Payment.TransactionId = transactionId;
        order.Payment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(transactionId);
    }
}
