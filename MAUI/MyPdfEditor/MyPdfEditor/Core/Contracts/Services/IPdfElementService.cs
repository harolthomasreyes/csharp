using MyPdfEditor.Core.Models.Enums;
using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface IPdfElementService
    {
        Task<PdfElement> CreateElementAsync(PdfElementTypeEnum type, double x, double y, double width, double height);
        Task<PdfElement> AddElementToDocumentAsync(PdfDocument document, PdfElement element);
        Task UpdateElementAsync(PdfDocument document, PdfElement element);
        Task RemoveElementAsync(PdfDocument document, string elementId);
        Task<PdfElement> GetElementAsync(PdfDocument document, string elementId);
        Task<IEnumerable<PdfElement>> GetAllElementsAsync(PdfDocument document);
        Task<bool> ValidateElementPositionAsync(PdfDocument document, PdfElement element);
        Task<bool> ElementExistsAsync(PdfDocument document, string elementId);
    }
}
