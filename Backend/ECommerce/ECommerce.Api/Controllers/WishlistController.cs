using ECommerce.Application.Features.Wishlist.Commands.AddToWishlist;
using ECommerce.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using ECommerce.Application.Features.Wishlist.Queries.GetWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get the current user's wishlist of furniture items.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlist(CancellationToken ct)
        {
            var result = await Sender.Send(new GetWishlistQuery(), ct);
            return Ok(result);
        }

        /// <summary>
        /// Add a furniture variant to the wishlist.
        /// </summary>
        [HttpPost("{variantId:int}")]
        public async Task<IActionResult> Add(int variantId, CancellationToken ct)
        {
            var result = await Sender.Send(new AddToWishlistCommand(variantId), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Remove a furniture item from the wishlist.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remove(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new RemoveFromWishlistCommand(id), ct);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
