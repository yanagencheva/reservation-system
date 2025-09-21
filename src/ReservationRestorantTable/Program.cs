using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reservation.Initializer;
using Reservation.Infrastructure;
using Reservation.Persistence;
using ReservationRestorantTable;
using ReservationRestorantTable.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
              })
              .ConfigureLogging((context, logging) =>
              {
                  logging.ClearProviders();
                  logging.AddConsole();

                  var logFolder = context.Configuration["Logging:LogFolder"];
                  if (string.IsNullOrWhiteSpace(logFolder))
                      logFolder = "Logs";

                  if (!Path.IsPathRooted(logFolder))
                      logFolder = Path.Combine(AppContext.BaseDirectory, logFolder);

                  if (!Directory.Exists(logFolder))
                      Directory.CreateDirectory(logFolder);

                  logging.AddProvider(new FileLoggerProvider(logFolder));
                  logging.SetMinimumLevel(LogLevel.Information);
              })
              .ConfigureServices((context, services) =>
              {
                  var configuration = context.Configuration;
                             
                  services.AddPersistenceServices(configuration);
                  services.AddInfrastructure(configuration);

                  services.AddSingleton<IResponseService, ResponseService>();
                  services.AddHostedService<WorkerService>();
              });

        var app = builder.Build();

        app.Initialize();

        await app.RunAsync();
    }
}
