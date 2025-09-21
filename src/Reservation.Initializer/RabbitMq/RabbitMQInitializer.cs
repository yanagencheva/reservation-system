using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Reservation.Initializer.RabbitMq;

public class RabbitMQInitializer
{
    public static Task Initialize(IServiceProvider services, IConfiguration configuration)
    {
        InitializeMessageQueues(services, configuration);

        return Task.CompletedTask;
    }

    private static void InitializeMessageQueues(IServiceProvider services, IConfiguration configuration)
    {
        var settings = services.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
        var logger = services.GetRequiredService<ILogger<RabbitMQConnection>>();
        var connection = new RabbitMQConnection(settings, logger);
        if (!connection.IsConnected)
        {
            connection.TryConnect();
        }

        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(
            exchange: settings.Exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null);
        foreach (var queueDef in Queues.AllQueues)
        {
            DeclareQueue(channel, settings.Exchange, queueDef.Key, queueDef.Value);
        }
    }

    private static void DeclareQueue(IModel channel, string exchange, string queue, IDictionary<string, object>? args = null)
    {
        channel.QueueDeclare(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            args);

        channel.QueueBind(queue, exchange, routingKey: queue, args);
    }
}
