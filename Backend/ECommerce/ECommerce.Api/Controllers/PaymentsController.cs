using ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent;
using ECommerce.Application.Features.Payments.Commands.UpdatePaymentStatus;
using ECommerce.Application.Features.Payments.Queries.GetPaymentByOrderId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Initiate a payment transaction for an order (Stripe).
        /// Returns the transaction/client-secret ID to use on the frontend.
        /// </summary>
        [HttpPost("create-intent/{orderId:int}")]
        public async Task<IActionResult> CreateIntent(int orderId, CancellationToken ct)
        {
            var result = await Sender.Send(new CreatePaymentIntentCommand(orderId), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get payment details for a specific order.
        /// </summary>
        [HttpGet("by-order/{orderId:int}")]
        public async Task<IActionResult> GetByOrder(int orderId, CancellationToken ct)
        {
            var result = await Sender.Send(new GetPaymentByOrderIdQuery(orderId), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Webhook endpoint - called by payment gateway to confirm payment status.
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] UpdatePaymentStatusCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
