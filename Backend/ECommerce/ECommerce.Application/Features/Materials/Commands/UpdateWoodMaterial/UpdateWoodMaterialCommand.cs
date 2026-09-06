using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Materials.Commands.UpdateWoodMaterial;

public record UpdateWoodMaterialCommand(
    int Id,
    string Name,
    decimal Thickness,
    string Unit,
    decimal UnitPrice,
    bool IsActive) : IRequest<Result<bool>>;

public class UpdateWoodMaterialCommandValidator : AbstractValidator<UpdateWoodMaterialCommand>
{
    public UpdateWoodMaterialCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Thickness).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}

public class UpdateWoodMaterialCommandHandler : IRequestHandler<UpdateWoodMaterialCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateWoodMaterialCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateWoodMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = await _context.WoodMaterials
            .FirstOrDefaultAsync(wm => wm.Id == request.Id, cancellationToken);

        if (material == null)
        {
            return Result<bool>.Failure("Wood material was not found.");
        }

        material.Name = request.Name.Trim();
        material.Thickness = request.Thickness;
        material.Unit = request.Unit.Trim();
        material.UnitPrice = request.UnitPrice;
        material.IsActive = request.IsActive;
        material.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
