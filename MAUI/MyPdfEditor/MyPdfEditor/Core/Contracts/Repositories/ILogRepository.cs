using MyPdfEditor.Core.Models.Enums.Logs;
using MyPdfEditor.Core.Models.Logs;

namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface ILogRepository
    {
        Task LogErrorAsync(string message, Exception exception = null);
        Task LogWarningAsync(string message);
        Task LogInformationAsync(string message);
        Task LogDebugAsync(string message);
        Task<IEnumerable<LogEntry>> GetLogsAsync(DateTime startDate, DateTime endDate, LogLevelEnum level = LogLevelEnum.All);
        Task ClearLogsAsync(DateTime olderThan);
    }
}
