namespace ECommerce.Application.Features.Settings.DTOs;

public record CompanySettingsDto(
    int Id,
    decimal CarpenterPercentage,
    decimal CompanyProfitPercentage,
    decimal DepositPercentage,
    DateTime EffectiveFrom,
    DateTime CreatedAt);
