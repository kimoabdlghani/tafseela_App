using ECommerce.Application.Features.Cart.Commands.AddToCart;
using ECommerce.Application.Features.Cart.Commands.ClearCart;
using ECommerce.Application.Features.Cart.Commands.RemoveFromCart;
using ECommerce.Application.Features.Cart.Commands.UpdateCartItemQuantity;
using ECommerce.Application.Features.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    /// <summary>
    /// Manages the shopping cart (basket) for the authenticated customer.
    /// Allows adding furniture designs with specific size variants.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get the current user's cart with all items and their furniture variants.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new GetCartQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Add a furniture variant (specific design + size) to the cart.
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update the quantity of a cart item.
        /// </summary>
        [HttpPut("items/{id:int}")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] int quantity, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new UpdateCartItemQuantityCommand(id, quantity), cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Remove a specific item from the cart.
        /// </summary>
        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> RemoveFromCart(int id, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new RemoveFromCartCommand(id), cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }

        /// <summary>
        /// Clear all items from the cart.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new ClearCartCommand(), cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
