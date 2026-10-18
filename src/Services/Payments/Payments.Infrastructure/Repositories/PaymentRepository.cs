using Microsoft.EntityFrameworkCore;
using Payments.Core.Entities;
using Payments.Core.Repositories;
using Payments.Infrastructure.Persistence;

namespace Payments.Infrastructure.Repositories;

public class PaymentRepository(PaymentsDbContext db) : IPaymentRepository
{
    public Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken);
    }
}
