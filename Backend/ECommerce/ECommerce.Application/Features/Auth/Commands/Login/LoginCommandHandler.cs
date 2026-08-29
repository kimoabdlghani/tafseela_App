using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Auth.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokensDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IApplicationDbContext context, 
        IIdentityService identityService, 
        ITokenService tokenService)
    {
        _context = context;
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            return Result<AuthTokensDto>.Failure("Invalid email or password.");
        }

        if (user.UserStatus == UserStatus.Banned || user.UserStatus == UserStatus.Suspended)
        {
            return Result<AuthTokensDto>.Failure("Your account has been suspended or banned. Please contact support.");
        }

        if (!user.EmailConfirmed)
        {
            return Result<AuthTokensDto>.Failure("Please verify your email address before logging in.");
        }

        var isPasswordValid = await _identityService.CheckPasswordAsync(request.Email, request.Password);
        
        if (!isPasswordValid)
        {
            return Result<AuthTokensDto>.Failure("Invalid email or password.");
        }

        var roles = await _identityService.GetUserRolesAsync(user.Id);
        var accessToken =await _tokenService.GenerateAccessTokenAsync(user,roles,cancellationToken);
        var refreshToken =await _tokenService.GenerateRefreshTokenAsync(user,cancellationToken);

        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var authTokens = new AuthTokensDto(accessToken, refreshToken);
        return Result<AuthTokensDto>.Success(authTokens);
    }
}