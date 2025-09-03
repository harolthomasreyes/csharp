using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface IPdfRepository
    {
        Task<PdfDocument> LoadDocumentAsync(string filePath);
        Task SaveDocumentAsync(PdfDocument document, string filePath);
        Task<PdfDocument> CreateNewDocumentAsync();
        Task<IEnumerable<string>> GetRecentDocumentsAsync();
        Task AddToRecentDocumentsAsync(string filePath);
    }
}
