using ECommerce.Application.Features.Reviews.Commands.CreateReview;
using ECommerce.Application.Features.Reviews.Commands.DeleteReview;
using ECommerce.Application.Features.Reviews.Commands.UpdateReview;
using ECommerce.Application.Features.Reviews.Queries.GetReviewsByProductId;
using ECommerce.Application.Features.Reviews.Queries.GetUserReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController(ISender sender) : ApiControllerBase(sender)
    {
        /// <summary>
        /// Get all reviews for a specific furniture product.
        /// </summary>
        [HttpGet("by-product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId, CancellationToken ct)
        {
            var result = await Sender.Send(new GetReviewsByProductIdQuery(productId), ct);
            return Ok(result);
        }

        /// <summary>
        /// Get all reviews written by the current user.
        /// </summary>
        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews(CancellationToken ct)
        {
            var result = await Sender.Send(new GetUserReviewsQuery(), ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateReviewCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new DeleteReviewCommand(id), ct);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
