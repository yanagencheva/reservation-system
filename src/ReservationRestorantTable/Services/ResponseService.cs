using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reservation.Common.Enums;
using Reservation.Common.Models;
using Reservation.Persistence;
using System.Text.Json;

namespace ReservationRestorantTable.Services;

public class ResponseService : IResponseService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResponseService(
        ILogger<ResponseService> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task ProcessMessage(string message)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider
                .GetRequiredService<IDataStore>();

            var reservation = JsonSerializer.Deserialize<Reservation.Persistence.Entities.Reservation>(message);

            dataStore.Add(reservation);
            await dataStore.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully processed reservation at {Time}. Message: {MessagePreview}",
                DateTimeOffset.UtcNow,
                message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process reservation message. Message: {MessagePreview}",
                message);
        }
    }
}
