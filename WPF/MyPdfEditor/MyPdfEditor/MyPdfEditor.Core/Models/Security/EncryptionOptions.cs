using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Security
{
    public class EncryptionOptions
    {
        public EncryptionType EncryptionType { get; set; } = EncryptionType.None;
        public string Password { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;
        public int KeyLength { get; set; } = 256;
        public bool AllowPrinting { get; set; } = true;
        public bool AllowModification { get; set; } = true;
        public bool AllowCopyContent { get; set; } = true;
        public bool AllowAnnotations { get; set; } = true;
        public bool AllowFormFilling { get; set; } = true;
        public bool AllowAccessibility { get; set; } = true;
        public bool AllowAssembly { get; set; } = true;

        public bool IsEncrypted => EncryptionType != EncryptionType.None;

        public bool ValidatePasswordStrength()
        {
            if (string.IsNullOrEmpty(Password)) return false;

            // Basic password validation
            return Password.Length >= 6 &&
                   Password.Length <= 32 &&
                   !Password.Contains(" ");
        }
    }
}