using ECommerce.Application.Common.Models;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Description,
    string? ImageUrl) : IRequest<Result<int>>;