using ECommerce.Application.Features.Admin.Dashboard.Queries.GetDashboardSummary;
using ECommerce.Application.Features.Admin.Users.Commands.RevokeUserTokens;
using ECommerce.Application.Features.Admin.Users.Commands.UpdateUserStatus;
using ECommerce.Application.Features.Admin.Users.Queries.GetAllUsers;
using ECommerce.Application.Features.Admin.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController(ISender sender) : ApiControllerBase(sender)
    {
        // ─── Dashboard ──────────────────────────────────────────────────
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(CancellationToken ct)
        {
            var result = await Sender.Send(new GetDashboardSummaryQuery(), ct);
            return Ok(result);
        }

        // ─── Users ──────────────────────────────────────────────────────
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetAllUsersQuery(searchTerm, isActive, pageNumber, pageSize), ct);
            return Ok(result);
        }

        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new GetUserByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPut("users/status")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("users/{userId:int}/revoke-tokens")]
        public async Task<IActionResult> RevokeTokens(int userId, CancellationToken ct)
        {
            var result = await Sender.Send(new RevokeUserTokensCommand(userId), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
