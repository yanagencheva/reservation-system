using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reservation.Initializer.RabbitMq;

namespace Reservation.Initializer;

public static class InitializerExtensions
{
    public static IHost Initialize(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var settings = configuration.GetSection(InitializerSettings.SectionName)
            .Get<InitializerSettings>();

        if (settings != null && settings.Enabled)
        {
            try
            {
                Task.WaitAll(
                   PersistenceInitializer.Initialize(services, settings!),
                   RabbitMQInitializer.Initialize(services, configuration)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while initializing app dependencies. {0}", ex);
            }
        }

        return app;
    }
}
