namespace Reservation.Initializer.RabbitMq;

public static class Queues
{
    public const string MESSAGES = "Messages_RabbitMq";
    public const string SUCCESSED = "Success_RabbitMq";
    public const string FAILED = "Fail_RabbitMq";
    public const string RESPONSE = "Response_RabbitMq";

    public static Dictionary<string, Dictionary<string, object>> AllQueues => new Dictionary<string, Dictionary<string, object>>()
    {
        { MESSAGES, null },
        { SUCCESSED, null },
        { FAILED, null },
        { RESPONSE, null }
    };
}