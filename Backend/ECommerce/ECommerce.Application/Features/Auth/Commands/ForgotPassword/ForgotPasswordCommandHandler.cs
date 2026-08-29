using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context, 
        IIdentityService identityService, 
        IEmailService emailService)
    {
        _context = context;
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user != null && user.Email != null)
        {
            var resetToken = await _identityService.GeneratePasswordResetTokenAsync(user.Email);

            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, cancellationToken);
        }

        return Result.Success();
    }
}