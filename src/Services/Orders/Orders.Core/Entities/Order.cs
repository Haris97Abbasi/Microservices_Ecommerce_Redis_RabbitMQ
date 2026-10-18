namespace Orders.Core.Entities;

public enum OrderStatus
{
    Pending,
    Paid
}

public class Order
{
    public Guid Id { get; set; }

    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public static Order Create(string productId, string productName, decimal unitPrice, int quantity) => new()
    {
        Id = Guid.NewGuid(),
        ProductId = productId,
        ProductName = productName,
        UnitPrice = unitPrice,
        Quantity = quantity,
        TotalAmount = unitPrice * quantity,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    public void MarkAsPaid(DateTime paidAt)
    {
        if (Status == OrderStatus.Paid)
            return;

        Status = OrderStatus.Paid;
        PaidAt = paidAt;
    }
}
