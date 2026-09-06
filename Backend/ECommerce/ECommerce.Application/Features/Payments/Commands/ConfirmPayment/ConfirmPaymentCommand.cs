using System.Text.Json;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Payments.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(
    int PaymentId,
    string? GatewayTransactionId = null) : IRequest<Result<bool>>;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmPaymentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment == null)
        {
            return Result<bool>.Failure("Payment record was not found.");
        }

        if (payment.Status == PaymentStatus.Paid)
        {
            return Result<bool>.Success(true); // Idempotent success
        }

        // 1. Mark Payment as Paid
        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.GatewayTransactionId))
        {
            payment.GatewayTransactionId = request.GatewayTransactionId;
        }

        // 2. Update Order Status to DepositPaid
        var order = payment.Order;
        order.Status = OrderStatus.DepositPaid;
        order.UpdatedAt = DateTime.UtcNow;

        var changedByUserId = _currentUserService.UserId ?? order.UserId;

        // 3. Automatically generate ProductionJobs for each OrderItem
        foreach (var item in order.OrderItems)
        {
            var existingJob = await _context.ProductionJobs
                .FirstOrDefaultAsync(j => j.OrderItemId == item.Id, cancellationToken);

            if (existingJob == null)
            {
                // Prepare production specifications snapshot for carpenter
                var specs = new
                {
                    ProductName = item.ProductNameSnapshot,
                    WoodMaterial = item.WoodMaterialNameSnapshot,
                    WoodColor = item.WoodColorNameSnapshot,
                    DimensionsJson = item.DimensionsSnapshot,
                    ComponentsJson = item.ComponentsSnapshot,
                    CarpenterPayout = item.CarpenterAmountSnapshot,
                    OrderNumber = order.OrderNumber
                };

                var job = new ProductionJob
                {
                    OrderId = order.Id,
                    OrderItemId = item.Id,
                    Status = ProductionJobStatus.Available,
                    ProductionSnapshot = JsonSerializer.Serialize(specs),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    StatusHistory = new List<ProductionJobStatusHistory>
                    {
                        new ProductionJobStatusHistory
                        {
                            Status = ProductionJobStatus.Available,
                            ChangedAt = DateTime.UtcNow,
                            ChangedByUserId = changedByUserId,
                            Notes = "Job automatically generated upon deposit payment confirmation.",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    }
                };

                _context.ProductionJobs.Add(job);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
