using ECommerce.Application.Features.Auth.Commands.ForgotPassword;
using ECommerce.Application.Features.Auth.Commands.Login;
using ECommerce.Application.Features.Auth.Commands.RefreshToken;
using ECommerce.Application.Features.Auth.Commands.Register;
using ECommerce.Application.Features.Auth.Commands.ResetPassword;
using ECommerce.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ISender sender) : ApiControllerBase(sender)
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
