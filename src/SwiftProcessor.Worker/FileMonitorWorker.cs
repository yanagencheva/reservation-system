using SwiftProcessor.Worker.Models;
using SwiftProcessor.Worker.Services;
using SwiftProcessor.Worker.Utils;

namespace SwiftProcessor.Worker;

public class FileMonitorWorker : BackgroundService
{
    private readonly ILogger<FileMonitorWorker> _logger;
    private readonly ISwiftMessageService _swiftMessageService;
    private readonly string _folderPath;
    private FileSystemWatcher? _watcher;

    private string SuccessfulFolder => Path.Combine(_folderPath, "successful");
    private string FailedFolder => Path.Combine(_folderPath, "failed");

    public FileMonitorWorker(
        ILogger<FileMonitorWorker> logger, 
        IConfiguration configuration,
        ISwiftMessageService swiftMessageService)
    {
        _logger = logger;
        _folderPath = configuration["MonitorSettings:FolderPath"]
                      ?? throw new ArgumentNullException("MonitorSettings:FolderPath must be configured.");
        _swiftMessageService = swiftMessageService;

        Directory.CreateDirectory(SuccessfulFolder);
        Directory.CreateDirectory(FailedFolder);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting file monitor for folder: {Folder}", _folderPath);

        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
            _logger.LogWarning("Folder did not exist. Created: {Folder}", _folderPath);
        }

        _watcher = new FileSystemWatcher(_folderPath, "*.TXT")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
        };

        _watcher.Created += OnNewFileDetected;
        _watcher.EnableRaisingEvents = true;

        return Task.CompletedTask;
    }

    private void OnNewFileDetected(object sender, FileSystemEventArgs e)
    {
        Task.Run(async () =>
        {
            string destinationFolder = FailedFolder; // assume failed until proven successful

            try
            {
                // Wait until file is fully written
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        using (var stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            break; // file is ready
                        }
                    }
                    catch (IOException)
                    {
                        await Task.Delay(500);
                    }
                }

                string content = await File.ReadAllTextAsync(e.FullPath);

                _logger.LogInformation("Processing file {FileName}. Content length: {Length}", e.Name, content.Length);

                SwiftMessage parsedMessage = SwiftMessageParser.Parse(content);

                if (parsedMessage != null)
                {
                    await _swiftMessageService.SaveMessageAsync(parsedMessage);
                    destinationFolder = SuccessfulFolder;
                    _logger.LogInformation("File {FileName} parsed and saved successfully.", e.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process file {FileName}. Moving to failed folder.", e.Name);
            }
            finally
            {
                try
                {
                    string destFile = Path.Combine(destinationFolder, Path.GetFileName(e.FullPath));

                    if (File.Exists(destFile))
                    {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(destFile);
                        string ext = Path.GetExtension(destFile);
                        destFile = Path.Combine(destinationFolder, $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
                    }

                    File.Move(e.FullPath, destFile);
                    _logger.LogInformation("Moved file {FileName} to {DestinationFolder}", e.Name, destinationFolder);
                }
                catch (Exception moveEx)
                {
                    _logger.LogError(moveEx, "Failed to move file {FileName} to {DestinationFolder}", e.Name, destinationFolder);
                }
            }
        });
    }

    public override void Dispose()
    {
        base.Dispose();
        _watcher?.Dispose();
    }
}