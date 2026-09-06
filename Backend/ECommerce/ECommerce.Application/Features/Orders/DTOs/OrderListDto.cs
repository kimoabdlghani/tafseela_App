namespace ECommerce.Application.Features.Orders.DTOs;

public record OrderListDto(
    int Id,
    string OrderNumber,
    string CustomerEmail,
    string Status,
    decimal ProductTotal,
    decimal DeliveryCost,
    decimal OrderTotal,
    decimal DepositAmount,
    int ItemsCount,
    DateTime CreatedAt);