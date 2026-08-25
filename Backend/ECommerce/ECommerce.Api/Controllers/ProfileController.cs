using ECommerce.Application.Features.Profile.Commands.UpdateProfile;
using ECommerce.Application.Features.Profile.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get the current user's profile.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var result = await Sender.Send(new GetProfileQuery(), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Update the current user's profile (name and phone number).
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProfileCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
