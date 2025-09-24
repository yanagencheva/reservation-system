using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reservation.Persistence;

namespace Reservation.Initializer;

public static class PersistenceInitializer
{
    public static ILogger _logger;

    public static async Task Initialize(
        IServiceProvider services,
        InitializerSettings settings)
    {
        var context = services.GetRequiredService<ReservationDbContext>();
        ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger(nameof(PersistenceInitializer));

        try
        {
            _logger.LogInformation("PersistenceInitializer initialization started.");

            _logger.LogInformation("Checking for database migrations...");
            await context.Database.MigrateAsync();
            _logger.LogInformation("All database migrations have been successfully applied.");

            _logger.LogInformation("PersistenceInitializer initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during PersistenceInitializer initialization or database migrations.");
            throw;
        }
    }
}