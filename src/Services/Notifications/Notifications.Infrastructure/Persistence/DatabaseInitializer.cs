using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Notifications.Infrastructure.Persistence;

public class DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger) : IHostedService
{
    private const int MaxAttempts = 12;
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

                var created = await db.Database.EnsureCreatedAsync(cancellationToken);
                logger.LogInformation(created ? "Notifications database created" : "Notifications database already exists");
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts && !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Database not ready (attempt {Attempt}/{Max}): {Error} - retrying in {Delay}s",
                    attempt, MaxAttempts, ex.Message, Delay.TotalSeconds);
                await Task.Delay(Delay, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
