namespace MyPdfEditor.Core.Contracts.Services
{
    public interface ILoggingService
    {
        Task LogErrorAsync(string message, Exception exception = null);
        Task LogWarningAsync(string message);
        Task LogInformationAsync(string message);
        Task LogDebugAsync(string message);
        Task LogTraceAsync(string message);
        Task<string> GetLogsAsync(DateTime startDate, DateTime endDate, LogLevel level = LogLevel.All);
        Task ClearLogsAsync();
    }

    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical,
        None
    }
}
