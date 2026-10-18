using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application.Dtos;
using Notifications.Application.Queries;

namespace Notifications.Api.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<NotificationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid? orderId, CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetNotificationsQuery(orderId), cancellationToken));
}
