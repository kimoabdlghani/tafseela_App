using ECommerce.Application.Features.Settings.Commands.UpdateCompanySettings;
using ECommerce.Application.Features.Settings.Queries.GetCompanySettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SettingsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get active company percentage settings (Carpenter %, Company Profit %, Deposit %).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSettings(CancellationToken ct)
        {
            var result = await Sender.Send(new GetCompanySettingsQuery(), ct);
            return Ok(result);
        }

        /// <summary>
        /// Update company settings — inserts an immutable new version effective immediately.
        /// Existing orders retain their historical snapshots.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateCompanySettingsCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
