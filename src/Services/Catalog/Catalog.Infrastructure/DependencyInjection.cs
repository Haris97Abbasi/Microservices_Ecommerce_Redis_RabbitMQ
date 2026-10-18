using Catalog.Core.Entities;
using System.Net.Http.Headers;
using Catalog.Core.Repositories;
using Catalog.Core.Services;
using Catalog.Infrastructure.Llm;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        services.Configure<LlmSettings>(configuration.GetSection(LlmSettings.SectionName));
        services.AddHttpClient<ILlmClient, OpenAiCompatibleLlmClient>((sp, client) =>
        {
            var llm = sp.GetRequiredService<IOptions<LlmSettings>>().Value;

            client.BaseAddress = new Uri(llm.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(llm.TimeoutSeconds);

            if (!string.IsNullOrWhiteSpace(llm.ApiKey))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", llm.ApiKey);
        });

        return services;
    }
}
