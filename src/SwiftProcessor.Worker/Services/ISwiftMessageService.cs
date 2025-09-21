using SwiftProcessor.Worker.Models;

namespace SwiftProcessor.Worker.Services;

public interface ISwiftMessageService
{
    Task SaveMessageAsync(SwiftMessage rawMessage);
}
