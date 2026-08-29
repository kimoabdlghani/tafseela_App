using ECommerce.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace ECommerce.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        
        await smtp.ConnectAsync(
            _configuration["EmailSettings:SmtpServer"], 
            int.Parse(_configuration["EmailSettings:Port"]!), 
            SecureSocketOptions.StartTls, 
            cancellationToken);

        await smtp.AuthenticateAsync(
            _configuration["EmailSettings:SenderEmail"], 
            _configuration["EmailSettings:Password"], 
            cancellationToken);

        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }

   public async Task SendVerificationEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var encodedToken = Uri.EscapeDataString(token);
        var verificationUrl = $"{_configuration["ClientUrl"]}/verify-email?token={encodedToken}";
        
        var body = $"<h1>Verify Your Email</h1><p>Click the link below to verify your email:</p><a href='{verificationUrl}'>Verify Email</a>";
        
        await SendEmailAsync(email, "Verify Your Email", body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var encodedToken = Uri.EscapeDataString(token);
        var resetUrl = $"{_configuration["ClientUrl"]}/reset-password?token={encodedToken}";
        
        var body = $"<h1>Reset Your Password</h1><p>Click the link below to reset your password:</p><a href='{resetUrl}'>Reset Password</a>";
        
        await SendEmailAsync(email, "Reset Your Password", body, cancellationToken);
    }
}