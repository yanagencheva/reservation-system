using Reservation.Persistence;
using SwiftProcessor.Worker;
using SwiftProcessor.Worker.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddPersistenceServices(configuration);

                services.AddHostedService<FileMonitorWorker>();
                services.AddSingleton<ISwiftMessageService, SwiftMessageService>();
            })
            .Build();

        await host.RunAsync();
    }
}