using Microsoft.EntityFrameworkCore;
using Payments.Core.Entities;

namespace Payments.Infrastructure.Persistence;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("payments");
            e.HasKey(p => p.Id);

            e.HasIndex(p => p.OrderId).IsUnique();

            e.Property(p => p.Amount).HasPrecision(18, 2);

            e.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        });
    }
}
