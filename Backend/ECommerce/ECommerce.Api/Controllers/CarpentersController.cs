using ECommerce.Application.Features.Carpenters.Commands.Apply;
using ECommerce.Application.Features.Carpenters.Commands.ReviewApplication;
using ECommerce.Application.Features.Carpenters.Commands.UpdateProfile;
using ECommerce.Application.Features.Carpenters.Queries.GetApplications;
using ECommerce.Application.Features.Carpenters.Queries.GetProfiles;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarpentersController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Any authenticated user applies to join the platform as a carpenter.
        /// </summary>
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyRequest? request, CancellationToken ct)
        {
            var result = await Sender.Send(new ApplyToBeCarpenterCommand(request?.Notes), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin reviews pending carpenter applications.
        /// </summary>
        [HttpGet("applications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApplications([FromQuery] CarpenterApplicationStatus? status = null, CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetCarpenterApplicationsQuery(status), ct);
            return Ok(result);
        }

        /// <summary>
        /// Admin approves or rejects a carpenter application (creates active profile and assigns role on approval).
        /// </summary>
        [HttpPost("applications/{id:int}/review")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReviewApplication(int id, [FromBody] ReviewApplicationRequest request, CancellationToken ct)
        {
            var result = await Sender.Send(new ReviewCarpenterApplicationCommand(id, request.IsApproved, request.Notes), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin gets list of all carpenter profiles with metrics and active status.
        /// </summary>
        [HttpGet("profiles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfiles([FromQuery] CarpenterStatus? status = null, CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetCarpenterProfilesQuery(status), ct);
            return Ok(result);
        }

        /// <summary>
        /// Admin updates carpenter profile limits (e.g. MaxActiveJobs) or status (Active, Suspended, Inactive).
        /// </summary>
        [HttpPut("profiles/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request, CancellationToken ct)
        {
            var result = await Sender.Send(new UpdateCarpenterProfileCommand(id, request.MaxActiveJobs, request.Status), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }

    public record ApplyRequest(string? Notes);
    public record ReviewApplicationRequest(bool IsApproved, string? Notes);
    public record UpdateProfileRequest(int? MaxActiveJobs, CarpenterStatus? Status);
}
