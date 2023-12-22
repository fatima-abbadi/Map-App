using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestApiJwt.Models;

public class SaleCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SaleCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var expiredSales = dbContext.Sales
                    .Where(s => s.StartDate.AddMinutes(s.DurationInMinutes) < DateTime.UtcNow)
                    .ToList();

                if (expiredSales.Any())
                {
                    foreach (var sale in expiredSales)
                    {
                        // Schedule cleanup for each sale
                        await ScheduleCleanup(sale);
                    }
                }
            }

            // Sleep for a period before checking again (adjust as needed)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ScheduleCleanup(Sale sale)
    {
        DateTime cleanupTime = sale.StartDate.AddMinutes(sale.DurationInMinutes);

        // Ensure a positive delay (in case the sale duration is already finished)
        TimeSpan delay = cleanupTime - DateTime.UtcNow;
        delay = delay > TimeSpan.Zero ? delay : TimeSpan.Zero;

        // Schedule cleanup using a timer
        Timer cleanupTimer = new Timer(async _ =>
        {
            using (var innerScope = _serviceProvider.CreateScope())
            {
                var innerDbContext = innerScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Query the database again using the sale's primary key
                var saleToDelete = await innerDbContext.Sales.FindAsync(sale.Id);

                if (saleToDelete != null)
                {
                    // Remove the sale from the database
                    innerDbContext.Sales.Remove(saleToDelete);
                    await innerDbContext.SaveChangesAsync();
                }
            }
        }, null, delay, Timeout.InfiniteTimeSpan);
    }
}
