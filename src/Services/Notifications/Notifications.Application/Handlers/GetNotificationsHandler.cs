using MediatR;
using Notifications.Application.Dtos;
using Notifications.Application.Queries;
using Notifications.Core.Repositories;

namespace Notifications.Application.Handlers;

public class GetNotificationsHandler(INotificationRepository repository)
    : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>
{
    public async Task<IReadOnlyList<NotificationDto>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken)
    {
        var notifications = await repository.GetAsync(query.OrderId, cancellationToken);
        return notifications.Select(n => n.ToDto()).ToList();
    }
}
