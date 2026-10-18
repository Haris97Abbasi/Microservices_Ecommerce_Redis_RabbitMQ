using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository(IMongoCollection<Product> products) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await products.Find(FilterDefinition<Product>.Empty).ToListAsync(cancellationToken);

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await products.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.Id = string.Empty;
        await products.InsertOneAsync(product, cancellationToken: cancellationToken);
        return product;
    }

    public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(product.Id, out _))
            return false;

        var result = await products.ReplaceOneAsync(p => p.Id == product.Id, product, cancellationToken: cancellationToken);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await products.DeleteOneAsync(p => p.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }
}
