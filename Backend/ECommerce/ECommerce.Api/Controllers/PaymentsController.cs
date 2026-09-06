using ECommerce.Application.Features.Payments.Commands.ConfirmPayment;
using ECommerce.Application.Features.Payments.Commands.CreateDepositPayment;
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
        /// Create a pending deposit payment intent for an order.
        /// </summary>
        [HttpPost("deposit")]
        public async Task<IActionResult> CreateDeposit([FromBody] CreateDepositPaymentCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Confirm receipt of payment — marks order DepositPaid and automatically spawns ProductionJobs for carpenters.
        /// </summary>
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
