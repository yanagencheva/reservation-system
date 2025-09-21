using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Net.Sockets;
using Polly;
using Polly.Retry;

namespace Reservation.Initializer.RabbitMq;

public class RabbitMQConnection
{
    private static readonly object SyncLocker = new();

    private bool _disposed;
    private IConnection _connection; 
    private readonly RetryPolicy _policy;
    private readonly ConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQConnection> _logger;

    public RabbitMQConnection(
        RabbitMQSettings settings,
        ILogger<RabbitMQConnection> logger)
    {
        if (string.IsNullOrEmpty(settings.ConnectionString))
        {
            throw new ArgumentException("RabbitMQ connectionString is required.", nameof(settings));
        }

        _logger = logger;
        _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(settings.ConnectionString),
        };

        _policy = Policy
            .Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetry(settings.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, span) =>
                {
                    _logger.LogWarning(exception, "Failed to connect to RabbitMQ. Retrying in {Delay} seconds.", span.TotalSeconds);
                });
    }

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    public bool TryConnect()
    {
        try
        {
            _connection = _connectionFactory.CreateConnection();
            _logger.LogInformation("RabbitMQ connection established successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection.");
            return false;
        }
    }

    public IModel CreateModel()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            throw new InvalidOperationException("RabbitMQ connection is not established. Cannot create channel.");
        }

        return _connection.CreateModel();
    }

    private void OnException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        _logger.LogError(e.Exception, "RabbitMQ exception occurred: {Message}", e.Exception.Message);
        TryConnect();
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("RabbitMQ connection is blocked: {Reason}", e.Reason);
        TryConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        if (_disposed) return;

        _logger.LogWarning("RabbitMQ connection was shut down: {ReplyText}", e.ReplyText);
        TryConnect();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _connection?.Dispose();
        }
    }
}
