using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Commands.CreateDepositPayment;

public record CreateDepositPaymentCommand(int OrderId) : IRequest<Result<PaymentIntentDto>>;

public record PaymentIntentDto(
    int PaymentId,
    int OrderId,
    string OrderNumber,
    decimal Amount,
    string GatewayTransactionId,
    string Status);

public class CreateDepositPaymentCommandHandler : IRequestHandler<CreateDepositPaymentCommand, Result<PaymentIntentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateDepositPaymentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaymentIntentDto>> Handle(
        CreateDepositPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<PaymentIntentDto>.Failure("User is not authenticated.");
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId.Value, cancellationToken);

        if (order == null)
        {
            return Result<PaymentIntentDto>.Failure("Order was not found.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result<PaymentIntentDto>.Failure($"Cannot pay deposit for order with status '{order.Status}'.");
        }

        // Generate transaction reference
        var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.DepositAmount,
            PaymentMethod = PaymentMethod.Online,
            Status = PaymentStatus.Pending,
            GatewayTransactionId = transactionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Notes = $"Deposit payment for order #{order.OrderNumber}"
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new PaymentIntentDto(
            payment.Id,
            order.Id,
            order.OrderNumber,
            payment.Amount,
            transactionId,
            payment.Status.ToString());

        return Result<PaymentIntentDto>.Success(dto);
    }
}
