namespace BuildingBlocks.Events;

public record PaymentProcessed(
    Guid OrderId,
    Guid PaymentId,
    bool Success,
    decimal Amount,
    DateTime ProcessedAt);
