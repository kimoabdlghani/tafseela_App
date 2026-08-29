using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Payments.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdQueryHandler : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetPaymentByOrderIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaymentDto>> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Result<PaymentDto>.Failure("Unauthorized");

        var payment = await _context.Payments
            .AsNoTracking()
            .Where(p => p.OrderId == request.OrderId && p.Order.UserId == userId)
            .Select(p => new PaymentDto(
                p.Id,
                p.OrderId,
                p.Amount,
                p.PaymentStatus.ToString(), 
                p.PaymentMethod,
                p.TransactionId ?? "",
                p.CreatedAt,
                p.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            return Result<PaymentDto>.Failure("Payment details not found or you do not have permission to view them.");
        }

        return Result<PaymentDto>.Success(payment);
    }
}