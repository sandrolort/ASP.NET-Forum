using Services.Contracts.Contracts;
using ILogger = Serilog.ILogger;

namespace Logging;

public class FileLogger : ILoggerService
{
    private static ILogger _logger = null!;
    
    public FileLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void LogInfo(string message) => _logger.Information(message);

    public void LogWarn(string message) => _logger.Warning(message);

    public void LogDebug(string message) => _logger.Debug(message);

    public void LogError(string message) => _logger.Error(message);
    
    public void LogFatal(string message) => _logger.Fatal(message);
}