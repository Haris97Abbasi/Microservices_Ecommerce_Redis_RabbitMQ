using BuildingBlocks.Events;
using MassTransit;
using MediatR;
using Orders.Application.Commands;

namespace Orders.Application.Consumers;

public class PaymentProcessedConsumer(IMediator mediator) : IConsumer<PaymentProcessed>
{
    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var message = context.Message;
        await mediator.Send(
            new MarkOrderAsPaidCommand(message.OrderId, message.PaymentId, message.Success, message.ProcessedAt),
            context.CancellationToken);
    }
}
