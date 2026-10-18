using BuildingBlocks.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Application.Commands;
using Payments.Application.Dtos;
using Payments.Core.Entities;
using Payments.Core.Repositories;

namespace Payments.Application.Handlers;

public class ProcessPaymentHandler(
    IPaymentRepository repository,
    IPublishEndpoint publishEndpoint,
    ILogger<ProcessPaymentHandler> logger) : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await repository.GetByOrderIdAsync(command.OrderId, cancellationToken);

        if (payment is null)
        {
            payment = Payment.CreateSimulatedSuccess(command.OrderId, command.Amount);
            await repository.AddAsync(payment, cancellationToken);
            logger.LogInformation("Payment {PaymentId} for order {OrderId} succeeded ({Amount})",
                payment.Id, payment.OrderId, payment.Amount);
        }
        else
        {
            logger.LogInformation("Order {OrderId} already has payment {PaymentId} - not charging again",
                payment.OrderId, payment.Id);
        }

        await publishEndpoint.Publish(
            new PaymentProcessed(
                payment.OrderId,
                payment.Id,
                payment.Status == PaymentStatus.Succeeded,
                payment.Amount,
                payment.ProcessedAt),
            cancellationToken);

        return payment.ToDto();
    }
}
