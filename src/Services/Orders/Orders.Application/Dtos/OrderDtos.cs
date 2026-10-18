using System.ComponentModel.DataAnnotations;
using Orders.Core.Entities;

namespace Orders.Application.Dtos;

public record CreateOrderRequest(
    [Required, StringLength(24, MinimumLength = 24)] string ProductId,
    [Range(1, 100)] int Quantity);

public record OrderDto(
    Guid Id,
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    DateTime? PaidAt);

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order) => new(
        order.Id, order.ProductId, order.ProductName, order.UnitPrice, order.Quantity,
        order.TotalAmount, order.Status.ToString(), order.CreatedAt, order.PaidAt);
}
