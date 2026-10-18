using MediatR;
using Notifications.Application.Dtos;

namespace Notifications.Application.Queries;

public record GetNotificationsQuery(Guid? OrderId) : IRequest<IReadOnlyList<NotificationDto>>;
