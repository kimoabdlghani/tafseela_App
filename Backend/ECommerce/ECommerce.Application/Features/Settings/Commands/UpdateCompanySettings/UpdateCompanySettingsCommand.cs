using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Settings.Commands.UpdateCompanySettings;

public record UpdateCompanySettingsCommand(
    decimal CarpenterPercentage,
    decimal CompanyProfitPercentage,
    decimal DepositPercentage) : IRequest<Result<int>>;

public class UpdateCompanySettingsCommandValidator : AbstractValidator<UpdateCompanySettingsCommand>
{
    public UpdateCompanySettingsCommandValidator()
    {
        RuleFor(x => x.CarpenterPercentage)
            .InclusiveBetween(0.01m, 1.00m)
            .WithMessage("Carpenter percentage must be between 1% (0.01) and 100% (1.00).");

        RuleFor(x => x.CompanyProfitPercentage)
            .InclusiveBetween(0.01m, 2.00m)
            .WithMessage("Company profit percentage must be between 1% (0.01) and 200% (2.00).");

        RuleFor(x => x.DepositPercentage)
            .InclusiveBetween(0.05m, 1.00m)
            .WithMessage("Deposit percentage must be between 5% (0.05) and 100% (1.00).");
    }
}

public class UpdateCompanySettingsCommandHandler : IRequestHandler<UpdateCompanySettingsCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanySettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(UpdateCompanySettingsCommand request, CancellationToken cancellationToken)
    {
        // Insert a new immutable version of company settings effective immediately
        var newSettings = new CompanySettings
        {
            CarpenterPercentage = request.CarpenterPercentage,
            CompanyProfitPercentage = request.CompanyProfitPercentage,
            DepositPercentage = request.DepositPercentage,
            EffectiveFrom = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CompanySettings.Add(newSettings);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(newSettings.Id);
    }
}
