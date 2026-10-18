using BuildingBlocks.Events;
using MassTransit;
using MediatR;
using Notifications.Application.Commands;

namespace Notifications.Application.Consumers;

public class PaymentProcessedConsumer(IMediator mediator) : IConsumer<PaymentProcessed>
{
    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var message = context.Message;
        await mediator.Send(
            new CreatePaymentNotificationCommand(message.OrderId, message.PaymentId, message.Success, message.Amount),
            context.CancellationToken);
    }
}
