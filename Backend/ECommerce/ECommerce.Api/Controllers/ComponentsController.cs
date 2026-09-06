using ECommerce.Application.Features.Components.Commands.CreateComponent;
using ECommerce.Application.Features.Components.Commands.UpdateComponent;
using ECommerce.Application.Features.Components.Queries.GetComponents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentsController(ISender sender) : ApiControllerBase(sender)
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true, CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetComponentsQuery(onlyActive), ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateComponentCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateComponentCommand command, CancellationToken ct)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in route does not match command ID.");
            }

            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
