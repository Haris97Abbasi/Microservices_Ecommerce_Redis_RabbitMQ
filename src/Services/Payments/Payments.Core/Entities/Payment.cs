namespace Payments.Core.Entities;

public enum PaymentStatus
{
    Succeeded,
    Failed
}

public class Payment
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime ProcessedAt { get; set; }

    public static Payment CreateSimulatedSuccess(Guid orderId, decimal amount) => new()
    {
        Id = Guid.NewGuid(),
        OrderId = orderId,
        Amount = amount,
        Status = PaymentStatus.Succeeded,
        ProcessedAt = DateTime.UtcNow
    };
}
