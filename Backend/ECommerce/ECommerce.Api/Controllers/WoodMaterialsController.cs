using ECommerce.Application.Features.Materials.Commands.AddWoodColor;
using ECommerce.Application.Features.Materials.Commands.CreateWoodMaterial;
using ECommerce.Application.Features.Materials.Commands.UpdateWoodMaterial;
using ECommerce.Application.Features.Materials.Queries.GetWoodMaterials;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WoodMaterialsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get all wood materials and their active colors.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true, CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetWoodMaterialsQuery(onlyActive), ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateWoodMaterialCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWoodMaterialCommand command, CancellationToken ct)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in route does not match command ID.");
            }

            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:int}/colors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddColor(int id, [FromBody] AddColorRequest request, CancellationToken ct)
        {
            var result = await Sender.Send(new AddWoodColorCommand(id, request.Name), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }

    public record AddColorRequest(string Name);
}
