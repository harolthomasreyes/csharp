using MyPdfEditor.Core.Models.Enums;
using PdfSharp.Pdf;
using System.Windows.Controls;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateDocumentStructureAsync(PdfDocument document);
        Task<bool> ValidateElementAsync(PdfElementTypeEnum element);
        Task<IEnumerable<ValidationError>> ValidateDocumentComplianceAsync(PdfDocument document);
        Task<bool> CheckDocumentPermissionsAsync(PdfDocument document);
        Task<bool> IsDocumentEncryptedAsync(PdfDocument document);
    }
}
