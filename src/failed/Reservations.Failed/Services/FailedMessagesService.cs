using Reservation.Persistence;
using Reservation.Persistence.Entities;

namespace Reservations.Failed.Services;

public class FailedMessagesService : IFailedMessagesService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FailedMessagesService(
        ILogger<FailedMessagesService> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task<FailedReservation> AddMessage(string message)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider
                .GetRequiredService<IDataStore>();

            var failedMessage = new FailedReservation
            {
                DT = DateTimeOffset.UtcNow,
                Raw_Request = message
            };

            dataStore.Add(failedMessage);
            await dataStore.SaveChangesAsync();

            _logger.LogInformation("Successfully added failed message at {Time}. Message preview: {Preview}",
                DateTimeOffset.UtcNow,
                message);

            return failedMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add failed message. Message preview: {Preview}",
                message);
        }

        return default;
    }
}
