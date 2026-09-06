namespace ECommerce.Application.Features.Cart.DTOs;

public record CartDto(List<CartItemDto> Items)
{
    public decimal TotalSellingPrice => Items.Where(i => i.IsValid).Sum(i => i.SellingPrice);
    public decimal TotalDepositRequired => Items.Where(i => i.IsValid).Sum(i => i.DepositAmount);
    public bool HasInvalidItems => Items.Any(i => !i.IsValid);
    public int ItemCount => Items.Count;
}