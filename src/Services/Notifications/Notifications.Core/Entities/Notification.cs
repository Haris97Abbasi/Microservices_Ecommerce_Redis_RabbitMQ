namespace Notifications.Core.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid PaymentId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public static Notification ForPayment(Guid orderId, Guid paymentId, bool succeeded, decimal amount) => new()
    {
        Id = Guid.NewGuid(),
        OrderId = orderId,
        PaymentId = paymentId,
        Type = succeeded ? "PaymentSucceeded" : "PaymentFailed",
        Message = succeeded
            ? $"Payment of {amount:0.00} USD for order {orderId} was successful. Thank you for your purchase!"
            : $"Payment of {amount:0.00} USD for order {orderId} failed. Please try again.",
        CreatedAt = DateTime.UtcNow
    };
}
