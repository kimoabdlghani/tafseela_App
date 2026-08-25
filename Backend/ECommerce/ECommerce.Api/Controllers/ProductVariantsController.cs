using ECommerce.Application.Features.ProductVariants.Commands.CreateProductVariant;
using ECommerce.Application.Features.ProductVariants.Commands.DeleteProductVariant;
using ECommerce.Application.Features.ProductVariants.Commands.UpdateProductVariant;
using ECommerce.Application.Features.ProductVariants.Commands.UpdateStock;
using ECommerce.Application.Features.ProductVariants.Queries.GetVariantsByProductId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductVariantsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get all available size/color variants for a furniture product.
        /// </summary>
        [HttpGet("by-product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId, CancellationToken ct)
        {
            var result = await Sender.Send(new GetVariantsByProductIdQuery(productId), ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductVariantCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateProductVariantCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int productId, CancellationToken ct)
        {
            var result = await Sender.Send(new DeleteProductVariantCommand(id, productId), ct);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
