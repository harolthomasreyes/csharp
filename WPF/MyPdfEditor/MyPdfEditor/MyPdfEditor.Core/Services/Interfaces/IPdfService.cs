// MyPdfEditor.Core/Services/Interfaces/IPdfService.cs
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyPdfEditor.Core.Services.Interfaces
{
    public interface IPdfService
    {
        // Document operations
        Task<PdfDocument> CreateNewDocumentAsync(double width, double height);
        Task<PdfDocument> LoadDocumentAsync(string filePath, string password = null);
        Task SaveDocumentAsync(PdfDocument document, string filePath, bool overwrite = true);
        Task<byte[]> ExportToBytesAsync(PdfDocument document);
        Task<PdfDocument> ImportFromBytesAsync(byte[] pdfData, string password = null);

        // Document validation
        Task<bool> ValidateDocumentStructureAsync(PdfDocument document);
        Task<bool> IsValidPdfFileAsync(string filePath);

        // Document merging and splitting
        Task<PdfDocument> MergeDocumentsAsync(IEnumerable<PdfDocument> documents);
        Task<PdfDocument> MergeDocumentsFromPathsAsync(IEnumerable<string> filePaths);
        Task<IEnumerable<PdfDocument>> SplitDocumentByPagesAsync(PdfDocument document, IEnumerable<int> pageNumbers);
        Task<IEnumerable<PdfDocument>> SplitDocumentByRangesAsync(PdfDocument document, IEnumerable<(int Start, int End)> ranges);

        // Page operations
        Task AddPageAsync(PdfDocument document, double width, double height);
        Task RemovePageAsync(PdfDocument document, int pageNumber);
        Task ReorderPagesAsync(PdfDocument document, IEnumerable<int> newOrder);

        // Security
        Task EncryptDocumentAsync(PdfDocument document, string password, EncryptionType encryptionType);
        Task DecryptDocumentAsync(PdfDocument document, string password);
        Task<bool> ValidatePasswordAsync(PdfDocument document, string password);
    }
}