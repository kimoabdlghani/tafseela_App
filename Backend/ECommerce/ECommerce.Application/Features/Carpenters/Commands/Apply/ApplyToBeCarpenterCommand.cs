using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carpenters.Commands.Apply;

public record ApplyToBeCarpenterCommand(string? Notes = null) : IRequest<Result<int>>;

public class ApplyToBeCarpenterCommandHandler : IRequestHandler<ApplyToBeCarpenterCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApplyToBeCarpenterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(ApplyToBeCarpenterCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<int>.Failure("User is not authenticated.");
        }

        // Check if user already has an active or pending profile
        var existingProfile = await _context.CarpenterProfiles
            .FirstOrDefaultAsync(cp => cp.UserId == userId.Value, cancellationToken);

        if (existingProfile != null)
        {
            return Result<int>.Failure("You already have a registered carpenter profile.");
        }

        var existingPendingApp = await _context.CarpenterApplications
            .FirstOrDefaultAsync(ca => ca.UserId == userId.Value && ca.Status == CarpenterApplicationStatus.Pending, cancellationToken);

        if (existingPendingApp != null)
        {
            return Result<int>.Failure("You already have a pending application awaiting Admin review.");
        }

        var application = new CarpenterApplication
        {
            UserId = userId.Value,
            Status = CarpenterApplicationStatus.Pending,
            SubmittedAt = DateTime.UtcNow,
            ReviewNotes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CarpenterApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(application.Id);
    }
}
