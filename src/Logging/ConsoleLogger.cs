using Services.Contracts.Contracts;

namespace Logging;

public class ConsoleLogger : ILoggerService
{
    public void LogInfo(string message) => Console.WriteLine($"Info: {message}");

    public void LogWarn(string message) => Console.WriteLine($"Warn: {message}");

    public void LogDebug(string message) => Console.WriteLine($"Debug: {message}");

    public void LogError(string message) => Console.WriteLine($"Error: {message}");

    public void LogFatal(string message) => Console.WriteLine($"Fatal: {message}");
}