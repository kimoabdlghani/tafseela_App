namespace ECommerce.Application.Features.Orders.DTOs;

public record OrderDetailDto(
    int Id,
    string OrderNumber,
    int UserId,
    string CustomerEmail,
    string Status,
    decimal ProductTotal,
    decimal DeliveryCost,
    decimal OrderTotal,
    decimal DepositAmount,
    decimal DepositPercentageSnapshot,
    string AddressSnapshot,
    DateTime CreatedAt,
    List<OrderItemDto> Items);