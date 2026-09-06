using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(int OrderId, string? Reason) : IRequest<Result<bool>>;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelOrderCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.Role == "Admin";

        var order = await _context.Orders
            .Include(o => o.ProductionJobs)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result<bool>.Failure("Order was not found.");
        }

        // If not admin, ensure the customer owns the order
        if (!isAdmin && order.UserId != userId)
        {
            return Result<bool>.Failure("You are not authorized to cancel this order.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return Result<bool>.Failure("Order is already cancelled.");
        }

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Shipping)
        {
            return Result<bool>.Failure("Cannot cancel order that has been shipped or delivered.");
        }

        // Apply cancellation: Deposit is non-refundable per specification
        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        // Cancel any associated production jobs
        foreach (var job in order.ProductionJobs)
        {
            if (job.Status != ProductionJobStatus.Delivered && job.Status != ProductionJobStatus.Cancelled)
            {
                job.Status = ProductionJobStatus.Cancelled;
                job.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
