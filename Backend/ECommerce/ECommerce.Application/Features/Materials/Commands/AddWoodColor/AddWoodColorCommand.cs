using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Materials.Commands.AddWoodColor;

public record AddWoodColorCommand(int WoodMaterialId, string Name) : IRequest<Result<int>>;

public class AddWoodColorCommandValidator : AbstractValidator<AddWoodColorCommand>
{
    public AddWoodColorCommandValidator()
    {
        RuleFor(x => x.WoodMaterialId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}

public class AddWoodColorCommandHandler : IRequestHandler<AddWoodColorCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddWoodColorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddWoodColorCommand request, CancellationToken cancellationToken)
    {
        var material = await _context.WoodMaterials
            .FirstOrDefaultAsync(wm => wm.Id == request.WoodMaterialId, cancellationToken);

        if (material == null)
        {
            return Result<int>.Failure("Wood material was not found.");
        }

        var color = new WoodColor
        {
            WoodMaterialId = material.Id,
            Name = request.Name.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.WoodColors.Add(color);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(color.Id);
    }
}
