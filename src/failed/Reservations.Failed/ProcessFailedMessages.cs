using Reservation.Infrastructure;
using Reservation.Initializer.RabbitMq;
using Reservation.Persistence;
using Reservations.Failed.Services;

namespace Reservations.Failed;

public class ProcessFailedMessages : BackgroundService
{
    private readonly ILogger<ProcessFailedMessages> _logger;
    private readonly IRabbitMqConsumerService _rabbitMqConsumer;
    private readonly IFailedMessagesService _messageService;

    public ProcessFailedMessages(
        ILogger<ProcessFailedMessages> logger,
        IRabbitMqConsumerService rabbitMqConsumer,
        IFailedMessagesService messageService
    )
    {
        _logger = logger;
        _rabbitMqConsumer = rabbitMqConsumer;
        _messageService = messageService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            _rabbitMqConsumer.StartConsumer(Queues.FAILED, ProcessFailedMessageAsync);
        }
    }

    private async Task ProcessFailedMessageAsync(string message)
    {
        await _messageService.AddMessage(message);
        _logger.LogInformation("Processing result message: {message}", message);
    }
}
