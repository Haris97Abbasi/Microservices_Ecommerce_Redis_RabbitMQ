using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Commands;
using Orders.Core.Repositories;

namespace Orders.Application.Handlers;

public class MarkOrderAsPaidHandler(IOrderRepository repository, ILogger<MarkOrderAsPaidHandler> logger)
    : IRequestHandler<MarkOrderAsPaidCommand, bool>
{
    public async Task<bool> Handle(MarkOrderAsPaidCommand command, CancellationToken cancellationToken)
    {
        if (!command.PaymentSucceeded)
        {
            logger.LogWarning("Payment {PaymentId} for order {OrderId} failed - order stays Pending",
                command.PaymentId, command.OrderId);
            return false;
        }

        var order = await repository.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("PaymentProcessed received for unknown order {OrderId} - ignored", command.OrderId);
            return false;
        }

        order.MarkAsPaid(command.PaidAt);
        await repository.SaveAsync(order, cancellationToken);

        logger.LogInformation("Order {OrderId} marked as Paid (payment {PaymentId})", order.Id, command.PaymentId);
        return true;
    }
}
