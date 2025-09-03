using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface ITemplateService
    {
        Task SaveTemplateAsync(PdfDocument document, string templateName, string category = null);
        Task<PdfDocument> LoadTemplateAsync(string templateName);
        Task<IEnumerable<TemplateInfo>> GetAvailableTemplatesAsync(string category = null);
        Task<bool> DeleteTemplateAsync(string templateName);
        Task<bool> TemplateExistsAsync(string templateName);
        Task<IEnumerable<string>> GetTemplateCategoriesAsync();
    }

    public class TemplateInfo
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public int PageCount { get; set; }
    }
}
