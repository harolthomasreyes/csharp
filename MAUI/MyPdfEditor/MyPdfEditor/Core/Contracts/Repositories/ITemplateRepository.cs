using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface ITemplateRepository
    {
        Task SaveTemplateAsync(PdfDocument template, string templateName);
        Task<PdfDocument> LoadTemplateAsync(string templateName);
        Task<IEnumerable<string>> GetAvailableTemplatesAsync();
        Task DeleteTemplateAsync(string templateName);
        Task<bool> TemplateExistsAsync(string templateName);
    }
}
