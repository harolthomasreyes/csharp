using MyPdfEditor.Core.Models.Enums;
using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface IExportRepository
    {
        Task ExportToImageAsync(PdfDocument document, string outputPath, ImageFormat format);
        Task ExportToHtmlAsync(PdfDocument document, string outputPath);
        Task ExportToTextAsync(PdfDocument document, string outputPath);
        Task<byte[]> ExportToByteArrayAsync(PdfDocument document);
    }
}
