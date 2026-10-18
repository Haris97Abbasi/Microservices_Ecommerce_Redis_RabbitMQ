namespace BuildingBlocks.Events;

public record OrderPlaced(
    Guid OrderId,
    string ProductId,
    int Quantity,
    decimal Amount,
    DateTime PlacedAt);
