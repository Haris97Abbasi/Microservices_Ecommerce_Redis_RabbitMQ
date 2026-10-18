using Catalog.Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Persistence;

public class CatalogSeeder(IMongoCollection<Product> products, ILogger<CatalogSeeder> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (await products.Find(FilterDefinition<Product>.Empty).AnyAsync(cancellationToken))
        {
            logger.LogInformation("Catalog already has products - skipping seed");
            return;
        }

        var seed = SeedProducts();
        await products.InsertManyAsync(seed, cancellationToken: cancellationToken);
        logger.LogInformation("Seeded {Count} products into the catalog", seed.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static List<Product> SeedProducts() =>
    [
        new() { Name = "Wireless Mouse", Category = "Electronics", Price = 24.99m, Stock = 150,
                Description = "Ergonomic 2.4 GHz wireless mouse with silent clicks and 18-month battery life." },
        new() { Name = "Mechanical Keyboard", Category = "Electronics", Price = 89.00m, Stock = 60,
                Description = "Compact 75% keyboard with hot-swappable brown switches and RGB backlight." },
        new() { Name = "Noise-Cancelling Headphones", Category = "Electronics", Price = 199.50m, Stock = 35,
                Description = "Over-ear Bluetooth headphones with active noise cancelling and 30-hour battery." },
        new() { Name = "Running Shoes", Category = "Sportswear", Price = 120.00m, Stock = 80,
                Description = "Lightweight road-running shoes with breathable mesh and cushioned sole." },
        new() { Name = "Yoga Mat", Category = "Sportswear", Price = 35.00m, Stock = 200,
                Description = "6 mm non-slip yoga mat made of eco-friendly TPE, includes carry strap." },
        new() { Name = "Stainless Steel Water Bottle", Category = "Home", Price = 18.75m, Stock = 300,
                Description = "750 ml double-wall insulated bottle; keeps drinks cold 24 h or hot 12 h." },
        new() { Name = "Espresso Machine", Category = "Home", Price = 349.99m, Stock = 12,
                Description = "15-bar pump espresso machine with steam wand for milk frothing." },
        new() { Name = "Clean Code (Book)", Category = "Books", Price = 32.40m, Stock = 45,
                Description = "Robert C. Martin's handbook of agile software craftsmanship." }
    ];
}
