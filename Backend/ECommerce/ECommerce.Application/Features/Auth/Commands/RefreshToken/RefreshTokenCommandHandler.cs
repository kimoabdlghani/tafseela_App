using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Auth.DTOs;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthTokensDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context, 
        ITokenService tokenService,
        IIdentityService identityService)
    {
        _context = context;
        _tokenService = tokenService;
        _identityService = identityService;
    }

    public async Task<Result<AuthTokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);
        if (userId == null)
        {
            return Result<AuthTokensDto>.Failure("Invalid access token.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || user.IsDeleted)
        {
            return Result<AuthTokensDto>.Failure("User is invalid or deactivated.");
        }

        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId&& !rt.IsRevoked , cancellationToken);

        if (existingRefreshToken == null)
        {
            return Result<AuthTokensDto>.Failure("Invalid refresh token.");
        }

        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            _context.RefreshTokens.Remove(existingRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Result<AuthTokensDto>.Failure("Refresh token has expired. Please log in again.");
        }

        var roles = await _identityService.GetUserRolesAsync(user.Id);
        var newAccessToken =await _tokenService.GenerateAccessTokenAsync(user,roles,cancellationToken);
        var newRefreshToken =await _tokenService.GenerateRefreshTokenAsync(user,cancellationToken);

        _context.RefreshTokens.Remove(existingRefreshToken);

        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7), 
            CreatedAt = DateTime.UtcNow
        };
        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync(cancellationToken);

        var authTokens = new AuthTokensDto(newAccessToken, newRefreshToken);
        return Result<AuthTokensDto>.Success(authTokens);
    }
}