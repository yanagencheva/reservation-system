using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reservation.Common.Models;
using Reservation.Infrastructure;
using Reservation.Initializer.RabbitMq;
using ReservationRestorantTable.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ReservationRestorantTable;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IRabbitMqConsumerService _rabbitMqConsumer;
    private readonly IResponseService _responseService;

    public WorkerService(
       ILogger<WorkerService> logger,
       IRabbitMqConsumerService rabbitMqConsumer,
       IResponseService responseService
    )
    {
        _logger = logger;
        _rabbitMqConsumer = rabbitMqConsumer;
        _responseService = responseService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WorkerService started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            _rabbitMqConsumer.StartConsumer(Queues.MESSAGES, ProcessReservationMessageAsync);

            _rabbitMqConsumer.StartConsumer(Queues.RESPONSE, ProcessResultMessageAsync);

            _logger.LogInformation("WorkerService is working at: {time}", DateTimeOffset.Now);
        }

        _logger.LogInformation("WorkerService stopping at: {time}", DateTimeOffset.Now);
    }

    private async Task ProcessReservationMessageAsync(string message)
    {
        try
        {
            var reservation = JsonSerializer.Deserialize<ReservationMessage>(message);
            if (reservation == null) return;

            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(reservation, new ValidationContext(reservation), validationResults, true);

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                    _logger.LogWarning("Validation error: {Error}", validationResult.ErrorMessage);

                await Task.Run(() => _rabbitMqConsumer.PublishMessage(message, Queues.FAILED));
                return;
            }

            await Task.Run(() => _rabbitMqConsumer.PublishMessage(message, Queues.SUCCESSED));
        }
        catch (Exception)
        {
            await Task.Run(() => _rabbitMqConsumer.PublishMessage(message, Queues.FAILED));
        }
    }

    private async Task ProcessResultMessageAsync(string message)
    {
        await _responseService.ProcessMessage(message);
        _logger.LogInformation("Processing result message: {message}", message);
    }
}