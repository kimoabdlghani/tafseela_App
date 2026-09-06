using ECommerce.Application.Features.Cart.Commands.AddToCart;
using ECommerce.Application.Features.Cart.Commands.ClearCart;
using ECommerce.Application.Features.Cart.Commands.RemoveFromCart;
using ECommerce.Application.Features.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController(ISender sender) : ApiControllerBase(sender)
    {
        [HttpGet]
        public async Task<IActionResult> GetCart(CancellationToken ct)
        {
            var result = await Sender.Send(new GetCartQuery(), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> RemoveItem(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new RemoveFromCartCommand(id), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart(CancellationToken ct)
        {
            var result = await Sender.Send(new ClearCartCommand(), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
