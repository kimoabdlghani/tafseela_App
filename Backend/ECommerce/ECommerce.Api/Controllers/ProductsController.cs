using ECommerce.Application.Features.Customization.Commands.SaveConfiguration;
using ECommerce.Application.Features.Customization.Queries.CalculatePrice;
using ECommerce.Application.Features.Products.Commands.CreateProduct;
using ECommerce.Application.Features.Products.Commands.DeleteProduct;
using ECommerce.Application.Features.Products.Commands.UpdateProduct;
using ECommerce.Application.Features.Products.Queries.GetProductById;
using ECommerce.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get paginated furniture products with optional filters (category, search, sort).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? sortBy = null,
            CancellationToken ct = default)
        {
            var result = await Sender.Send(
                new GetProductsQuery(pageNumber, pageSize, searchTerm, categoryId, sortBy), ct);
            return Ok(result);
        }

        /// <summary>
        /// Get a single furniture product with its full customization options (dimensions, allowed woods, colors, components).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new GetProductByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Live calculation of customized product price based on selected dimensions, wood, and color.
        /// </summary>
        [HttpPost("calculate-price")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculateProductPriceQuery query, CancellationToken ct)
        {
            var result = await Sender.Send(query, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Save a customized product configuration.
        /// </summary>
        [HttpPost("configurations")]
        public async Task<IActionResult> SaveConfiguration([FromBody] SaveProductConfigurationCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateProductCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new DeleteProductCommand(id), ct);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
