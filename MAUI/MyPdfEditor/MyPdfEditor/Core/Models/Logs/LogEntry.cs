using Microsoft.Extensions.Logging;

namespace MyPdfEditor.Core.Models.Logs
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
