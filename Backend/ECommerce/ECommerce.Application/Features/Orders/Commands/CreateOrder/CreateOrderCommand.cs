using ECommerce.Application.Common.Models;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(int AddressId, string? PaymentMethod = null) : IRequest<Result<int>>;