// MyPdfEditor.Core/Services/Interfaces/ISecurityService.cs
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Enums;
using MyPdfEditor.Core.Models.Security;
using System.Threading.Tasks;

namespace MyPdfEditor.Core.Services.Interfaces
{
    public interface ISecurityService
    {
        // Encryption
        Task EncryptDocumentAsync(PdfDocument document, EncryptionOptions options);
        Task DecryptDocumentAsync(PdfDocument document, string password);
        Task<bool> CanDecryptDocumentAsync(PdfDocument document, string password);

        // Password management
        Task<bool> ValidatePasswordStrengthAsync(string password);
        Task<bool> ChangeDocumentPasswordAsync(PdfDocument document, string currentPassword, string newPassword);
        Task<bool> RemovePasswordProtectionAsync(PdfDocument document, string currentPassword);

        // Permissions
        Task SetDocumentPermissionsAsync(PdfDocument document, Permissions permissions);
        Task<Permissions> GetDocumentPermissionsAsync(PdfDocument document);
        Task<bool> HasPermissionAsync(PdfDocument document, PermissionType permissionType);
        Task<bool> CanPrintDocumentAsync(PdfDocument document);
        Task<bool> CanModifyDocumentAsync(PdfDocument document);
        Task<bool> CanCopyContentAsync(PdfDocument document);

        // Security validation
        Task<SecurityAuditResult> AuditDocumentSecurityAsync(PdfDocument document);
        Task<bool> IsDocumentEncryptedAsync(PdfDocument document);
        Task<int> GetEncryptionLevelAsync(PdfDocument document);

        // Certificate-based security (future extension)
        Task<bool> AddDigitalSignatureAsync(PdfDocument document, byte[] certificateData, string password);
        Task<bool> VerifyDigitalSignatureAsync(PdfDocument document);
    }

    public enum PermissionType
    {
        Print,
        Modify,
        Copy,
        Annotate,
        FillForms,
        ExtractContent,
        Assemble
    }

    public class SecurityAuditResult
    {
        public bool IsEncrypted { get; set; }
        public EncryptionType EncryptionType { get; set; }
        public int KeyLength { get; set; }
        public bool HasStrongPassword { get; set; }
        public Permissions EffectivePermissions { get; set; }
        public List<string> SecurityWarnings { get; set; } = new List<string>();
        public List<string> SecurityRecommendations { get; set; } = new List<string>();
    }
}