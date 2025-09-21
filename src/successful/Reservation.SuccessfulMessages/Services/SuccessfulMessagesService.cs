using Reservation.Common.Enums;
using Reservation.Common.Models;
using Reservation.Persistence;
using System.Text.Json;

namespace Reservation.SuccessfulMessages.Services;

public class SuccessfulMessagesService : ISuccessfulMessagesService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SuccessfulMessagesService(
        ILogger<SuccessfulMessagesService> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<ResultMessage> AddMessage(string message)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider
                .GetRequiredService<IDataStore>();

            var id = Guid.NewGuid();
            var dt = DateTimeOffset.UtcNow;
            var rawRequest = JsonSerializer.Serialize(message);

            await dataStore.ExecuteStoredProcedureNonQueryAsync(
                "insert_successful_reservation",
                id,
                dt,
                rawRequest
            );

            _logger.LogInformation("Successfully added message at {Time}. Message preview: {Preview}",
                DateTimeOffset.UtcNow,
                message);

            return new ResultMessage { 
                Raw_Request = message,
                DT = dt,
                Validation_Result = ValidationResults.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add successful message. Message preview: {Preview}",
                message);
        }

        return null;
    }
}
