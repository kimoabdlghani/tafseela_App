using ECommerce.Application.Features.ProductionJobs.Commands.AssignJob;
using ECommerce.Application.Features.ProductionJobs.Commands.CompleteJob;
using ECommerce.Application.Features.ProductionJobs.Commands.InspectJob;
using ECommerce.Application.Features.ProductionJobs.Commands.StartJob;
using ECommerce.Application.Features.ProductionJobs.Queries.GetAvailableJobs;
using ECommerce.Application.Features.ProductionJobs.Queries.GetCarpenterEarnings;
using ECommerce.Application.Features.ProductionJobs.Queries.GetCarpenterJobs;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductionJobsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Carpenters view all available unassigned jobs ready for manufacturing.
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = "Carpenter,Admin")]
        public async Task<IActionResult> GetAvailable(CancellationToken ct)
        {
            var result = await Sender.Send(new GetAvailableJobsQuery(), ct);
            return Ok(result);
        }

        /// <summary>
        /// Admin assigns a job to a carpenter (validates active status and max job limits).
        /// </summary>
        [HttpPost("{id:int}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignJobRequest request, CancellationToken ct)
        {
            var result = await Sender.Send(new AssignJobCommand(id, request.CarpenterProfileId), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Carpenter starts working on an assigned job.
        /// </summary>
        [HttpPost("{id:int}/start")]
        [Authorize(Roles = "Carpenter")]
        public async Task<IActionResult> Start(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new StartJobCommand(id), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Carpenter marks a job as completed.
        /// </summary>
        [HttpPost("{id:int}/complete")]
        [Authorize(Roles = "Carpenter")]
        public async Task<IActionResult> Complete(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new CompleteJobCommand(id), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin inspects the carpenter's completed work (Approved or ReworkRequired).
        /// </summary>
        [HttpPost("{id:int}/inspect")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Inspect(int id, [FromBody] InspectJobRequest request, CancellationToken ct)
        {
            var result = await Sender.Send(new InspectJobCommand(id, request.IsApproved, request.ReworkNotes), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Carpenter retrieves their own list of jobs.
        /// </summary>
        [HttpGet("my-jobs")]
        [Authorize(Roles = "Carpenter")]
        public async Task<IActionResult> GetMyJobs([FromQuery] ProductionJobStatus? status = null, CancellationToken ct = default)
        {
            var result = await Sender.Send(new GetCarpenterJobsQuery(status), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Carpenter retrieves their performance metrics and earnings summary.
        /// </summary>
        [HttpGet("earnings")]
        [Authorize(Roles = "Carpenter")]
        public async Task<IActionResult> GetEarnings(CancellationToken ct)
        {
            var result = await Sender.Send(new GetCarpenterEarningsQuery(), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }

    public record AssignJobRequest(int CarpenterProfileId);
    public record InspectJobRequest(bool IsApproved, string? ReworkNotes = null);
}
