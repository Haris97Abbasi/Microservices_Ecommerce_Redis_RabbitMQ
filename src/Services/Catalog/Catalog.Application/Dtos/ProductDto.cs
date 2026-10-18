using Catalog.Core.Entities;

namespace Catalog.Application.Dtos;

public record ProductDto(
    string Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    int Stock);

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.Name, product.Description, product.Category, product.Price, product.Stock);
}
