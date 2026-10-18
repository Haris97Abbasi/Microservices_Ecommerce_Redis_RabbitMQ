using Notifications.Core.Entities;

namespace Notifications.Application.Dtos;

public record NotificationDto(Guid Id, Guid OrderId, Guid PaymentId, string Type, string Message, DateTime CreatedAt);

public static class NotificationMappingExtensions
{
    public static NotificationDto ToDto(this Notification n) =>
        new(n.Id, n.OrderId, n.PaymentId, n.Type, n.Message, n.CreatedAt);
}
