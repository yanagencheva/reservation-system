using Reservation.Common.Models;
using Reservation.Infrastructure;
using Reservation.Initializer.RabbitMq;
using Reservation.SuccessfulMessages.Services;
using System.Text.Json;

namespace Reservation.SuccessfulMessages;

public class ProcessSuccessfulMessages : BackgroundService
{
    private readonly ILogger<ProcessSuccessfulMessages> _logger;
    private readonly IRabbitMqConsumerService _rabbitMqConsumer;
    private readonly ISuccessfulMessagesService _messageService;

    public ProcessSuccessfulMessages(
        ILogger<ProcessSuccessfulMessages> logger,
        IRabbitMqConsumerService rabbitMqConsumer,
        ISuccessfulMessagesService messageService
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
            _rabbitMqConsumer.StartConsumer(Queues.SUCCESSED, ProcessSuccessfulMessageAsync);
        }
    }

    private async Task ProcessSuccessfulMessageAsync(string message)
    {
        var result = await _messageService.AddMessage(message);
        _logger.LogInformation("Processing result message: {message}", message);
        if(result != null)
        {
            var messageWithValidation = JsonSerializer.Serialize(result);
            _rabbitMqConsumer.PublishMessage(messageWithValidation, Queues.RESPONSE);
            _logger.LogInformation("Publish message - {message} to: {queueName}", messageWithValidation, Queues.RESPONSE);
        }
    }
}
