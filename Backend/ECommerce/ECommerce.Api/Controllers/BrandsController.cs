using ECommerce.Application.Features.Brands.Commands.CreateBrand;
using ECommerce.Application.Features.Brands.Commands.DeleteBrand;
using ECommerce.Application.Features.Brands.Commands.UpdateBrand;
using ECommerce.Application.Features.Brands.Queries.GetBrandById;
using ECommerce.Application.Features.Brands.Queries.GetBrands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController(ISender sender)
    : ApiControllerBase(sender)
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateBrandCommand command)
        {
            var result = await Sender.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var result = await Sender.Send(new GetBrandByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Sender.Send(new GetBrandsQuery());
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            UpdateBrandCommand command)
        {
            var result = await Sender.Send(command);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await Sender.Send(new DeleteBrandCommand(id));

            return success.IsSuccess
                ? NoContent()
                : NotFound();
        }
    }
}
