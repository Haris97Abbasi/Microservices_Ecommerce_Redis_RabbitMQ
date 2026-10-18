using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ProductMapping.Register();

        var settings = configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
                       ?? new MongoDbSettings();

        services.AddSingleton<IMongoClient>(_ => new MongoClient(settings.ConnectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>()
            .GetDatabase(settings.DatabaseName)
            .GetCollection<Product>(settings.ProductsCollectionName));

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddHostedService<CatalogSeeder>();

        return services;
    }
}
