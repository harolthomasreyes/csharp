namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface IPdfElementRepository
    {
        Task<PdfElement> GetElementByIdAsync(string documentId, string elementId);
        Task<IEnumerable<PdfElement>> GetAllElementsAsync(string documentId);
        Task<PdfElement> AddElementAsync(string documentId, PdfElement element);
        Task UpdateElementAsync(string documentId, PdfElement element);
        Task DeleteElementAsync(string documentId, string elementId);
        Task<bool> ElementExistsAsync(string documentId, string elementId);
    }
}
