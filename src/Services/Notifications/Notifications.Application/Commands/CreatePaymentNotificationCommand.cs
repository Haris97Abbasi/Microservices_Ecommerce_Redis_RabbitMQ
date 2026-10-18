using MediatR;

namespace Notifications.Application.Commands;

public record CreatePaymentNotificationCommand(Guid OrderId, Guid PaymentId, bool Succeeded, decimal Amount)
    : IRequest<bool>;
