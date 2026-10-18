using MediatR;
using Orders.Application.Dtos;
using Orders.Application.Queries;
using Orders.Core.Repositories;

namespace Orders.Application.Handlers;

public class GetOrderByIdHandler(IOrderRepository repository) : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(query.Id, cancellationToken);
        return order?.ToDto();
    }
}
