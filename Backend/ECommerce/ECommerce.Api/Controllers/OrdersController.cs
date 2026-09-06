using ECommerce.Application.Features.Orders.Commands.CancelOrder;
using ECommerce.Application.Features.Orders.Commands.Checkout;
using ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using ECommerce.Application.Features.Orders.Queries.GetAdminOrderById;
using ECommerce.Application.Features.Orders.Queries.GetAdminOrders;
using ECommerce.Application.Features.Orders.Queries.GetCustomerOrderById;
using ECommerce.Application.Features.Orders.Queries.GetCustomerOrders;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Customer checkout — converts active cart items into an order with frozen snapshots.
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Customer retrieves their order history.
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(CancellationToken ct)
        {
            var result = await Sender.Send(new GetCustomerOrdersQuery(), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Customer retrieves detailed view of a specific order.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new GetCustomerOrderByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Customer or Admin cancels an order (deposit is non-refundable).
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderRequest? request, CancellationToken ct)
        {
            var result = await Sender.Send(new CancelOrderCommand(id, request?.Reason), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin gets list of all orders with optional status or search filter.
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] string? searchTerm = null,
            CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetAdminOrdersQuery(status, searchTerm), ct);
            return Ok(result);
        }

        /// <summary>
        /// Admin gets full detail of an order.
        /// </summary>
        [HttpGet("admin/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdAdmin(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new GetAdminOrderByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Admin updates the lifecycle status of an order.
        /// </summary>
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
        {
            var result = await Sender.Send(new UpdateOrderStatusCommand(id, request.Status), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }

    public record CancelOrderRequest(string? Reason);
    public record UpdateOrderStatusRequest(OrderStatus Status);
}
