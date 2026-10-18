using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.Dtos;
using Orders.Application.Exceptions;
using Orders.Application.Queries;
using Orders.Core.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<OrderDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await mediator.Send(new CreateOrderCommand(request.ProductId, request.Quantity), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (ProductNotFoundException ex)
        {
            return Problem(title: "Product not found", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (CatalogUnavailableException)
        {
            return Problem(
                title: "Catalog unavailable",
                detail: "The product could not be verified because the Catalog API is unreachable. Try again later.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }
}
