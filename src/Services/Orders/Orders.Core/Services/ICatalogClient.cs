namespace Orders.Core.Services;

public record CatalogProduct(string Id, string Name, decimal Price, int Stock);

public interface ICatalogClient
{
    Task<CatalogProduct?> GetProductAsync(string productId, CancellationToken cancellationToken = default);
}

public class CatalogUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);
