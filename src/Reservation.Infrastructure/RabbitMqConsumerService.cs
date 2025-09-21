using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reservation.Initializer.RabbitMq;

namespace Reservation.Infrastructure;

public class RabbitMqConsumerService : IRabbitMqConsumerService
{
    private readonly RabbitMQConnection _connection;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    public RabbitMqConsumerService(
        RabbitMQConnection connection, 
        ILogger<RabbitMqConsumerService> logger
    )
    {
        _connection = connection;
        _logger = logger;
    }

    public void StartConsumer(string queueName, Func<string, Task> handleMessage)
    {
        if (!_connection.TryConnect())
        {
            _logger.LogError("Cannot start consumer because RabbitMQ connection could not be established.");
            return;
        }

        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (ch, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            await handleMessage(body);
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    public void PublishMessage(string message, string queueName)
    {
        using var channel = _connection.CreateModel();

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body
        );

        _logger.LogInformation("Message published to queue: {queue}", queueName);
    }
}
