using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Components.Commands.CreateComponent;

public record CreateComponentCommand(
    string Name,
    ComponentType Type,
    string Unit,
    decimal UnitPrice) : IRequest<Result<int>>;

public class CreateComponentCommandValidator : AbstractValidator<CreateComponentCommand>
{
    public CreateComponentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}

public class CreateComponentCommandHandler : IRequestHandler<CreateComponentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateComponentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateComponentCommand request, CancellationToken cancellationToken)
    {
        var component = new Component
        {
            Name = request.Name.Trim(),
            Type = request.Type,
            Unit = request.Unit.Trim(),
            UnitPrice = request.UnitPrice,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Components.Add(component);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(component.Id);
    }
}
