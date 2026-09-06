using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Materials.Commands.CreateWoodMaterial;

public record CreateWoodMaterialCommand(
    string Name,
    decimal Thickness,
    string Unit,
    decimal UnitPrice,
    List<string>? InitialColors = null) : IRequest<Result<int>>;

public class CreateWoodMaterialCommandValidator : AbstractValidator<CreateWoodMaterialCommand>
{
    public CreateWoodMaterialCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Thickness).GreaterThan(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}

public class CreateWoodMaterialCommandHandler : IRequestHandler<CreateWoodMaterialCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateWoodMaterialCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateWoodMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = new WoodMaterial
        {
            Name = request.Name.Trim(),
            Thickness = request.Thickness,
            Unit = request.Unit.Trim(),
            UnitPrice = request.UnitPrice,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Colors = new List<WoodColor>()
        };

        if (request.InitialColors != null)
        {
            foreach (var colorName in request.InitialColors.Where(c => !string.IsNullOrWhiteSpace(c)))
            {
                material.Colors.Add(new WoodColor
                {
                    Name = colorName.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        _context.WoodMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(material.Id);
    }
}
