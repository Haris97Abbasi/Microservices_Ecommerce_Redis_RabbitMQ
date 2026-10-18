using Microsoft.EntityFrameworkCore;
using Notifications.Core.Entities;

namespace Notifications.Infrastructure.Persistence;

public class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("Notifications");
            e.HasKey(n => n.Id);

            e.HasIndex(n => n.PaymentId).IsUnique();

            e.HasIndex(n => n.OrderId);

            e.Property(n => n.Type).HasMaxLength(50);
            e.Property(n => n.Message).HasMaxLength(500);

            e.Property(n => n.CreatedAt)
             .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
    }
}
