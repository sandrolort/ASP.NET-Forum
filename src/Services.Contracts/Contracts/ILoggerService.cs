﻿namespace Services.Contracts.Contracts;

public interface ILoggerService
{
    void LogInfo(string message);
    void LogWarn(string message);
    void LogDebug(string message);
    void LogError(string message);
    void LogFatal(string message);
}