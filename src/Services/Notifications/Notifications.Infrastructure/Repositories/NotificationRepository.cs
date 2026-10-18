using Microsoft.EntityFrameworkCore;
using Notifications.Core.Entities;
using Notifications.Core.Repositories;
using Notifications.Infrastructure.Persistence;

namespace Notifications.Infrastructure.Repositories;

public class NotificationRepository(NotificationsDbContext db) : INotificationRepository
{
    public Task<bool> ExistsForPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default) =>
        db.Notifications.AnyAsync(n => n.PaymentId == paymentId, cancellationToken);

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetAsync(Guid? orderId, CancellationToken cancellationToken = default)
    {
        var query = db.Notifications.AsNoTracking();

        if (orderId is not null)
            query = query.Where(n => n.OrderId == orderId);

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync(cancellationToken);
    }
}
