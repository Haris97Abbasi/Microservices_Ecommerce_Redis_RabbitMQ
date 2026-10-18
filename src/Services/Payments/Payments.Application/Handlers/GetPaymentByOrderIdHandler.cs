using MediatR;
using Payments.Application.Dtos;
using Payments.Application.Queries;
using Payments.Core.Repositories;

namespace Payments.Application.Handlers;

public class GetPaymentByOrderIdHandler(IPaymentRepository repository)
    : IRequestHandler<GetPaymentByOrderIdQuery, PaymentDto?>
{
    public async Task<PaymentDto?> Handle(GetPaymentByOrderIdQuery query, CancellationToken cancellationToken)
    {
        var payment = await repository.GetByOrderIdAsync(query.OrderId, cancellationToken);
        return payment?.ToDto();
    }
}
