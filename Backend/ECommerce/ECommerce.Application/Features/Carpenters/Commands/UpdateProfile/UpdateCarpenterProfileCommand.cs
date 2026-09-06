using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Carpenters.Commands.UpdateProfile;

public record UpdateCarpenterProfileCommand(
    int CarpenterProfileId,
    int? MaxActiveJobs,
    CarpenterStatus? Status) : IRequest<Result<bool>>;

public class UpdateCarpenterProfileCommandValidator : AbstractValidator<UpdateCarpenterProfileCommand>
{
    public UpdateCarpenterProfileCommandValidator()
    {
        RuleFor(x => x.CarpenterProfileId).GreaterThan(0);
        When(x => x.MaxActiveJobs.HasValue, () =>
        {
            RuleFor(x => x.MaxActiveJobs!.Value).GreaterThan(0).WithMessage("Max active jobs must be at least 1.");
        });
    }
}

public class UpdateCarpenterProfileCommandHandler : IRequestHandler<UpdateCarpenterProfileCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateCarpenterProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateCarpenterProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.CarpenterProfiles
            .FirstOrDefaultAsync(cp => cp.Id == request.CarpenterProfileId, cancellationToken);

        if (profile == null)
        {
            return Result<bool>.Failure("Carpenter profile was not found.");
        }

        if (request.MaxActiveJobs.HasValue)
        {
            profile.MaxActiveJobs = request.MaxActiveJobs.Value;
        }

        if (request.Status.HasValue)
        {
            profile.Status = request.Status.Value;
        }

        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
