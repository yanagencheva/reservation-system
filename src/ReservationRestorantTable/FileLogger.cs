using Microsoft.Extensions.Logging;

namespace ReservationRestorantTable;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
        TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}";
        lock (_lock)
        {
            File.AppendAllText(_filePath, message + Environment.NewLine);
        }
    }
}

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _folderPath;

    public FileLoggerProvider(string folderPath)
    {
        _folderPath = folderPath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        var logFile = Path.Combine(_folderPath, $"{DateTime.Now:yyyyMMdd}.log");
        return new FileLogger(logFile);
    }

    public void Dispose() { }
}
