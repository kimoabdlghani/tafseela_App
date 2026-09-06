using ECommerce.Application.Common.Models;
using MediatR;

namespace ECommerce.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    int CategoryId,
    bool IsActive) : IRequest<Result>;