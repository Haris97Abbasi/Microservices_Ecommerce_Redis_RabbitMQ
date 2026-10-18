using System.Net;
using System.Net.Http.Json;
using Orders.Core.Services;

namespace Orders.Infrastructure.Catalog;

public class CatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<CatalogProduct?> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync(
                $"catalog/products/{Uri.EscapeDataString(productId)}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
                throw new CatalogUnavailableException($"Catalog API returned {(int)response.StatusCode}");

            return await response.Content.ReadFromJsonAsync<CatalogProduct>(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new CatalogUnavailableException($"Could not reach Catalog API: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new CatalogUnavailableException("Catalog API timed out", ex);
        }
    }
}
