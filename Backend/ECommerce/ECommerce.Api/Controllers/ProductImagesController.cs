using ECommerce.Application.Features.ProductImages.Commands.AddProductImage;
using ECommerce.Application.Features.ProductImages.Commands.DeleteProductImage;
using ECommerce.Application.Features.ProductImages.Commands.SetPrimaryProductImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductImagesController(ISender sender) : ApiControllerBase(sender)
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddProductImageCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int productId, CancellationToken ct)
        {
            var result = await Sender.Send(new DeleteProductImageCommand(id, productId), ct);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }

        [HttpPut("{id:int}/set-primary")]
        public async Task<IActionResult> SetPrimary(int id, [FromQuery] int productId, CancellationToken ct)
        {
            var result = await Sender.Send(new SetPrimaryProductImageCommand(id, productId), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
