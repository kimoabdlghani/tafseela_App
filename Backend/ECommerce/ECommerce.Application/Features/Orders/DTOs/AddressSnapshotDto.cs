namespace ECommerce.Application.Features.Orders.DTOs;

public record AddressSnapshotDto(
    string Street,
    string City,
    string? Building,
    string PostalCode,
    string Country,
    string? Apartment,
    string? Floor,
    string? Notes);
