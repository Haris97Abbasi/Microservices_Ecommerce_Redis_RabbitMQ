using Orders.Application.Dtos;
using MediatR;

namespace Orders.Application.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;
