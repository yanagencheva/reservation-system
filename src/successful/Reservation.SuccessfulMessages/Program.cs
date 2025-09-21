using Reservation.Infrastructure;
using Reservation.Initializer;
using Reservation.Persistence;
using Reservation.SuccessfulMessages;
using Reservation.SuccessfulMessages.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
              })
              .ConfigureServices((context, services) =>
              {
                  var configuration = context.Configuration;

                  services.AddPersistenceServices(configuration);
                  services.AddInfrastructure(configuration);

                  services.AddHostedService<ProcessSuccessfulMessages>();
                  services.AddSingleton<ISuccessfulMessagesService, SuccessfulMessagesService>();

              });

        var app = builder.Build();

        app.Initialize();

        await app.RunAsync();
    }
}
