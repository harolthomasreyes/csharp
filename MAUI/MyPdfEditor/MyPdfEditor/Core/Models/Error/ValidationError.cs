using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Error
{
    public class ValidationError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public ValidationSeverityEnum Severity { get; set; }
        public string ElementId { get; set; }
    }
}
