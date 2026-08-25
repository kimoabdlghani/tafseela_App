using ECommerce.Application.Features.Categories.Commands.CreateCategory;
using ECommerce.Application.Features.Categories.Commands.DeleteCategory;
using ECommerce.Application.Features.Categories.Commands.UpdateCategory;
using ECommerce.Application.Features.Categories.Queries.GetCategories;
using ECommerce.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ISender sender) : ApiControllerBase(sender)
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await Sender.Send(new GetCategoriesQuery(), ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new GetCategoryByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await Sender.Send(new DeleteCategoryCommand(id), ct);
            return result.IsSuccess ? NoContent() : BadRequest(result);
        }
    }
}
