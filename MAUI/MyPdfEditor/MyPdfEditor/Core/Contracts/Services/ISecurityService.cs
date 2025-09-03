using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface ISecurityService
    {
        Task EncryptDocumentAsync(PdfDocument document, string password, EncryptionLevel level);
        Task DecryptDocumentAsync(PdfDocument document, string password);
        Task<bool> IsDocumentEncryptedAsync(PdfDocument document);
        Task SetDocumentPermissionsAsync(PdfDocument document, DocumentPermissions permissions);
        Task<DocumentPermissions> GetDocumentPermissionsAsync(PdfDocument document);
        Task<bool> ValidatePasswordStrengthAsync(string password);
    }

    public class DocumentPermissions
    {
        public bool AllowPrinting { get; set; } = true;
        public bool AllowModification { get; set; } = true;
        public bool AllowCopyContent { get; set; } = true;
        public bool AllowAccessibility { get; set; } = true;
        public bool AllowFormFilling { get; set; } = true;
        public bool AllowAssembly { get; set; } = true;
    }

    public enum EncryptionLevel
    {
        Low,    // 40-bit RC4
        Medium, // 128-bit RC4
        High,   // 128-bit AES
        Maximum // 256-bit AES
    }
}
