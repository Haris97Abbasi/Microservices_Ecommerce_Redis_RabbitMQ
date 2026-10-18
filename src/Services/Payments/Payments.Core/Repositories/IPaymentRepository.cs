using Payments.Core.Entities;

namespace Payments.Core.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
}
