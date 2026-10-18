using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Dtos;
using Payments.Application.Queries;

namespace Payments.Api.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType<PaymentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var payment = await mediator.Send(new GetPaymentByOrderIdQuery(orderId), cancellationToken);
        return payment is null ? NotFound() : Ok(payment);
    }
}
