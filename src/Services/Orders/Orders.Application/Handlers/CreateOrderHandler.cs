using BuildingBlocks.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Commands;
using Orders.Application.Dtos;
using Orders.Application.Exceptions;
using Orders.Core.Entities;
using Orders.Core.Repositories;
using Orders.Core.Services;

namespace Orders.Application.Handlers;

public class CreateOrderHandler(
    ICatalogClient catalogClient,
    IOrderRepository repository,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateOrderHandler> logger) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var product = await catalogClient.GetProductAsync(command.ProductId, cancellationToken)
                      ?? throw new ProductNotFoundException(command.ProductId);

        var order = Order.Create(product.Id, product.Name, product.Price, command.Quantity);
        await repository.SaveAsync(order, cancellationToken);

        await publishEndpoint.Publish(
            new OrderPlaced(order.Id, order.ProductId, order.Quantity, order.TotalAmount, order.CreatedAt),
            cancellationToken);

        logger.LogInformation("Order {OrderId} placed for {Quantity} x {Product}, total {Total}",
            order.Id, order.Quantity, order.ProductName, order.TotalAmount);

        return order.ToDto();
    }
}
