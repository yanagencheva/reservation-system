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

        await context.Database.MigrateAsync();
    }
}