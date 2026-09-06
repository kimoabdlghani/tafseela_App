using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Settings.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Settings.Queries.GetCompanySettings;

public record GetCompanySettingsQuery : IRequest<Result<CompanySettingsDto>>;

public class GetCompanySettingsQueryHandler : IRequestHandler<GetCompanySettingsQuery, Result<CompanySettingsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompanySettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CompanySettingsDto>> Handle(
        GetCompanySettingsQuery request,
        CancellationToken cancellationToken)
    {
        var settings = await _context.CompanySettings
            .AsNoTracking()
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
        {
            // Default baseline settings if none seeded yet
            return Result<CompanySettingsDto>.Success(new CompanySettingsDto(
                Id: 0,
                CarpenterPercentage: 0.30m,
                CompanyProfitPercentage: 0.20m,
                DepositPercentage: 0.30m,
                EffectiveFrom: DateTime.UtcNow,
                CreatedAt: DateTime.UtcNow));
        }

        return Result<CompanySettingsDto>.Success(new CompanySettingsDto(
            settings.Id,
            settings.CarpenterPercentage,
            settings.CompanyProfitPercentage,
            settings.DepositPercentage,
            settings.EffectiveFrom,
            settings.CreatedAt));
    }
}
