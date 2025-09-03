using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface IPdfService
    {
        Task<PdfDocument> CreateNewDocumentAsync();
        Task<PdfDocument> LoadDocumentAsync(string filePath);
        Task SaveDocumentAsync(PdfDocument document, string filePath);
        Task<PdfDocument> ImportDocumentAsync(byte[] fileData);
        Task<byte[]> ExportDocumentAsync(PdfDocument document);
        Task<bool> ValidateDocumentAsync(PdfDocument document);
        Task<PdfDocument> MergeDocumentsAsync(IEnumerable<PdfDocument> documents);
        Task<PdfDocument> SplitDocumentAsync(PdfDocument document, int startPage, int endPage);
    }
}
