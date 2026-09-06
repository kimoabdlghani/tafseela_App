using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Components.Commands.UpdateComponent;

public record UpdateComponentCommand(
    int Id,
    string Name,
    ComponentType Type,
    string Unit,
    decimal UnitPrice,
    bool IsActive) : IRequest<Result<bool>>;

public class UpdateComponentCommandValidator : AbstractValidator<UpdateComponentCommand>
{
    public UpdateComponentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}

public class UpdateComponentCommandHandler : IRequestHandler<UpdateComponentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateComponentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateComponentCommand request, CancellationToken cancellationToken)
    {
        var component = await _context.Components
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (component == null)
        {
            return Result<bool>.Failure("Component was not found.");
        }

        component.Name = request.Name.Trim();
        component.Type = request.Type;
        component.Unit = request.Unit.Trim();
        component.UnitPrice = request.UnitPrice;
        component.IsActive = request.IsActive;
        component.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
