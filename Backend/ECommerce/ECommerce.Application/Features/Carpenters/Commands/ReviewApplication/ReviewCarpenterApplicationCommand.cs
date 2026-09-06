using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carpenters.Commands.ReviewApplication;

public record ReviewCarpenterApplicationCommand(
    int ApplicationId,
    bool IsApproved,
    string? Notes = null) : IRequest<Result<bool>>;

public class ReviewCarpenterApplicationCommandValidator : AbstractValidator<ReviewCarpenterApplicationCommand>
{
    public ReviewCarpenterApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);
    }
}

public class ReviewCarpenterApplicationCommandHandler : IRequestHandler<ReviewCarpenterApplicationCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;

    public ReviewCarpenterApplicationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<User> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(
        ReviewCarpenterApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var adminUserId = _currentUserService.UserId ?? 0;

        var application = await _context.CarpenterApplications
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (application == null)
        {
            return Result<bool>.Failure("Carpenter application was not found.");
        }

        if (application.Status != CarpenterApplicationStatus.Pending)
        {
            return Result<bool>.Failure($"Application is already '{application.Status}'.");
        }

        application.ReviewedAt = DateTime.UtcNow;
        application.ReviewedByAdminId = adminUserId;
        application.ReviewNotes = request.Notes;
        application.UpdatedAt = DateTime.UtcNow;

        if (request.IsApproved)
        {
            application.Status = CarpenterApplicationStatus.Approved;

            // Ensure profile exists
            var existingProfile = await _context.CarpenterProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == application.UserId, cancellationToken);

            if (existingProfile == null)
            {
                var profile = new CarpenterProfile
                {
                    UserId = application.UserId,
                    MaxActiveJobs = 1, // Default initial capacity for new carpenters
                    Status = CarpenterStatus.Active,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CarpenterProfiles.Add(profile);
            }
            else
            {
                existingProfile.Status = CarpenterStatus.Active;
                existingProfile.UpdatedAt = DateTime.UtcNow;
            }

            // Assign "Carpenter" Role
            if (application.User != null)
            {
                var isInRole = await _userManager.IsInRoleAsync(application.User, "Carpenter");
                if (!isInRole)
                {
                    await _userManager.AddToRoleAsync(application.User, "Carpenter");
                }
            }
        }
        else
        {
            application.Status = CarpenterApplicationStatus.Rejected;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
