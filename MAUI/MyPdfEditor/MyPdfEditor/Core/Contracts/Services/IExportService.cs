using MyPdfEditor.Core.Models.Enums;
using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface IExportService
    {
        Task ExportToPdfAAsync(PdfDocument document, string outputPath);
        Task ExportToImagesAsync(PdfDocument document, string outputDirectory, ImageExportOptions options);
        Task<string> ExportToHtmlAsync(PdfDocument document, HtmlExportOptions options);
        Task<string> ExportToTextAsync(PdfDocument document, TextExportOptions options);
        Task<byte[]> ExportToByteArrayAsync(PdfDocument document, ExportFormat format);
    }

    public class ImageExportOptions
    {
        public int Dpi { get; set; } = 300;
        public ImageFormat Format { get; set; } = ImageFormat.Png;
        public int Quality { get; set; } = 100;
    }

    public class HtmlExportOptions
    {
        public bool IncludeImages { get; set; } = true;
        public bool IncludeStyles { get; set; } = true;
        public string CssStyle { get; set; }
    }

    public class TextExportOptions
    {
        public bool PreserveLayout { get; set; } = false;
        public bool IncludeMetadata { get; set; } = true;
    }

    public enum ExportFormat
    {
        Pdf,
        PdfA,
        PdfX
    }
}
