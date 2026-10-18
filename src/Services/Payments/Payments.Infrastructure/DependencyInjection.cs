using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Core.Repositories;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Repositories;

namespace Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentsDb")
                               ?? throw new InvalidOperationException("Connection string 'PaymentsDb' is missing");

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(5)));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddHostedService<DatabaseInitializer>();

        return services;
    }
}
