using MediatR;

namespace Orders.Application.Commands;

public record MarkOrderAsPaidCommand(Guid OrderId, Guid PaymentId, bool PaymentSucceeded, DateTime PaidAt)
    : IRequest<bool>;
