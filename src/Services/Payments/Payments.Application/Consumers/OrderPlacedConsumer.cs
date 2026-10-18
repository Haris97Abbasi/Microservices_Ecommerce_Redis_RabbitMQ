using BuildingBlocks.Events;
using MassTransit;
using MediatR;
using Payments.Application.Commands;

namespace Payments.Application.Consumers;

public class OrderPlacedConsumer(IMediator mediator) : IConsumer<OrderPlaced>
{
    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var message = context.Message;
        await mediator.Send(new ProcessPaymentCommand(message.OrderId, message.Amount), context.CancellationToken);
    }
}
