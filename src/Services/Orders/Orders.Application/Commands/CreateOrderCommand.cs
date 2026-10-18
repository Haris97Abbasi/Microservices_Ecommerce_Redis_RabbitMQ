using Orders.Application.Dtos;
using MediatR;

namespace Orders.Application.Commands;

public record CreateOrderCommand(string ProductId, int Quantity) : IRequest<OrderDto>;
