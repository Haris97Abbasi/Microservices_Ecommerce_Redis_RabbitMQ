using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Notifications.Infrastructure.Persistence;

public class DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var created = await db.Database.EnsureCreatedAsync(cancellationToken);
        logger.LogInformation(created ? "Notifications database created" : "Notifications database already exists");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
