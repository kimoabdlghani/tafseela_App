using ECommerce.Application.Features.Orders.Commands.CancelOrder;
using ECommerce.Application.Features.Orders.Commands.CreateOrder;
using ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus;
using ECommerce.Application.Features.Orders.Queries.GetOrderById;
using ECommerce.Application.Features.Orders.Queries.GetOrders;
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
        /// Get all orders for the current customer.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyOrders(CancellationToken ct)
        {
            var result = await Sender.Send(new GetOrdersQuery(), ct);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific order with its full details.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new GetOrderByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Convert current cart to an order. Cart items are moved to order items.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cancel a pending order.
        /// </summary>
        [HttpPost("{orderId:int}/cancel")]
        public async Task<IActionResult> Cancel(int orderId, CancellationToken ct)
        {
            var result = await Sender.Send(new CancelOrderCommand(orderId), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin: Update order status (Processing, Shipped, Delivered, Cancelled).
        /// </summary>
        [HttpPut("status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
