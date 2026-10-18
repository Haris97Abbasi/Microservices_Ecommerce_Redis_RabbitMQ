using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Commands;
using Notifications.Core.Entities;
using Notifications.Core.Repositories;

namespace Notifications.Application.Handlers;

public class CreatePaymentNotificationHandler(
    INotificationRepository repository,
    ILogger<CreatePaymentNotificationHandler> logger) : IRequestHandler<CreatePaymentNotificationCommand, bool>
{
    public async Task<bool> Handle(CreatePaymentNotificationCommand command, CancellationToken cancellationToken)
    {
        if (await repository.ExistsForPaymentAsync(command.PaymentId, cancellationToken))
        {
            logger.LogInformation("Notification for payment {PaymentId} already exists - skipped", command.PaymentId);
            return false;
        }

        var notification = Notification.ForPayment(command.OrderId, command.PaymentId, command.Succeeded, command.Amount);
        await repository.AddAsync(notification, cancellationToken);

        logger.LogInformation("Notification stored for order {OrderId}: {Message}", notification.OrderId, notification.Message);
        return true;
    }
}
