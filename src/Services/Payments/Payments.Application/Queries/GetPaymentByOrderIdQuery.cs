using MediatR;
using Payments.Application.Dtos;

namespace Payments.Application.Queries;

public record GetPaymentByOrderIdQuery(Guid OrderId) : IRequest<PaymentDto?>;
