namespace Orders.Application.Exceptions;

public class ProductNotFoundException(string productId)
    : Exception($"Product '{productId}' was not found in the catalog")
{
    public string ProductId { get; } = productId;
}
