using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orders.Core.Repositories;
using Orders.Core.Services;
using Orders.Infrastructure.Catalog;
using Orders.Infrastructure.Repositories;
using StackExchange.Redis;

namespace Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(redisConnection);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped<IOrderRepository, RedisOrderRepository>();

        services.Configure<CatalogApiSettings>(configuration.GetSection(CatalogApiSettings.SectionName));
        services.AddHttpClient<ICatalogClient, CatalogClient>((sp, client) =>
        {
            var catalog = sp.GetRequiredService<IOptions<CatalogApiSettings>>().Value;
            client.BaseAddress = new Uri(catalog.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(catalog.TimeoutSeconds);
        });

        return services;
    }
}
