namespace Reservation.Infrastructure;

public interface IRabbitMqConsumerService
{
    void StartConsumer(string queueName, Func<string, Task> processMessageAsync);
    void PublishMessage(string message, string queueName);
}
