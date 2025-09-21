namespace Reservation.Initializer.RabbitMq;

public class RabbitMQSettings
{
    public string ConnectionString { get; set; }
    public string Exchange { get; set; }
    public int RetryCount { get; set; } = 5;
}