using Notifications.Core.Entities;

namespace Notifications.Core.Repositories;

public interface INotificationRepository
{
    Task<bool> ExistsForPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);

    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetAsync(Guid? orderId, CancellationToken cancellationToken = default);
}
