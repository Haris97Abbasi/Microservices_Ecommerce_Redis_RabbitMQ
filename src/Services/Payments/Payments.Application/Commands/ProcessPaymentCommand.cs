using MediatR;
using Payments.Application.Dtos;

namespace Payments.Application.Commands;

public record ProcessPaymentCommand(Guid OrderId, decimal Amount) : IRequest<PaymentDto>;
