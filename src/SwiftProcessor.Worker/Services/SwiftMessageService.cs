using Reservation.Persistence;
using SwiftProcessor.Worker.Models;
using SwiftProcessor.Worker.Utils;

namespace SwiftProcessor.Worker.Services;

public class SwiftMessageService : ISwiftMessageService
{
    private readonly ILogger<SwiftMessageService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SwiftMessageService(
        ILogger<SwiftMessageService> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task SaveMessageAsync(SwiftMessage rawMessage)
    {
        _logger.LogInformation("Starting to process incoming SWIFT message.");

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider
                .GetRequiredService<IDataStore>();

            var entity = SwiftMessageMapper.ToEntity(rawMessage);

            _logger.LogInformation(
                "Parsed SWIFT message. TransactionReference={TransactionReference}, IsMt103Plus={IsMt103Plus}",
                entity.TransactionReference ?? "N/A",
                entity.IsMt103Plus);

            dataStore.Add(entity);
            await dataStore.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully saved SWIFT message with TransactionReference={TransactionReference} to database at {TimeUtc}",
                entity.TransactionReference ?? "N/A",
                DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process and save SWIFT message: {Preview}", rawMessage);
            throw; 
        }
    }
}