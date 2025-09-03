using System.Security.Cryptography;
using System.Text;
using PdfSharp.Pdf;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Security;
using MyPdfEditor.Core.Services.Interfaces;
using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class SecurityService : ISecurityService
    {
        public async Task EncryptDocumentAsync(PdfDocument document, EncryptionOptions options)
        {
            await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));
                if (options == null) throw new ArgumentNullException(nameof(options));

                if (!options.ValidatePasswordStrength())
                    throw new ArgumentException("Password does not meet strength requirements");

                document.Security.EncryptionType = options.EncryptionType;
                document.Security.Password = options.Password;
                document.Security.OwnerPassword = string.IsNullOrEmpty(options.OwnerPassword) ?
                    options.Password : options.OwnerPassword;
                document.Security.KeyLength = options.KeyLength;

                // Set permissions
                document.Security.AllowPrinting = options.AllowPrinting;
                document.Security.AllowModification = options.AllowModification;
                document.Security.AllowCopyContent = options.AllowCopyContent;
                document.Security.AllowAnnotations = options.AllowAnnotations;
                document.Security.AllowFormFilling = options.AllowFormFilling;
                document.Security.AllowAccessibility = options.AllowAccessibility;
                document.Security.AllowAssembly = options.AllowAssembly;

                document.IsModified = true;
            });
        }

        public async Task DecryptDocumentAsync(PdfDocument document, string password)
        {
            await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));

                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Password is required for decryption");

                // For PDFSharp-based implementation, this would involve opening with password
                // and then saving without encryption
                document.Security.EncryptionType = EncryptionType.None;
                document.Security.Password = string.Empty;
                document.Security.OwnerPassword = string.Empty;
                document.IsModified = true;
            });
        }

        public async Task<bool> CanDecryptDocumentAsync(PdfDocument document, string password)
        {
            return await Task.Run(() =>
            {
                if (document == null || !document.Security.IsEncrypted)
                    return true;

                return document.Security.Password == password ||
                       document.Security.OwnerPassword == password;
            });
        }

        public async Task<bool> ValidatePasswordStrengthAsync(string password)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(password))
                    return false;

                // Minimum length requirement
                if (password.Length < 6)
                    return false;

                // Maximum length requirement
                if (password.Length > 32)
                    return false;

                // Check for spaces
                if (password.Contains(" "))
                    return false;

                // Check for at least one uppercase letter
                if (!password.Any(char.IsUpper))
                    return false;

                // Check for at least one lowercase letter
                if (!password.Any(char.IsLower))
                    return false;

                // Check for at least one digit
                if (!password.Any(char.IsDigit))
                    return false;

                return true;
            });
        }

        public async Task<bool> ChangeDocumentPasswordAsync(PdfDocument document, string currentPassword, string newPassword)
        {
            return await Task.Run(() =>
            {
                if (document == null) return false;

                if (document.Security.IsEncrypted)
                {
                    // Verify current password
                    if (document.Security.Password != currentPassword &&
                        document.Security.OwnerPassword != currentPassword)
                        return false;
                }

                if (!ValidatePasswordStrengthAsync(newPassword).Result)
                    return false;

                document.Security.Password = newPassword;
                if (string.IsNullOrEmpty(document.Security.OwnerPassword))
                {
                    document.Security.OwnerPassword = newPassword;
                }
                document.IsModified = true;

                return true;
            });
        }

        public async Task<bool> RemovePasswordProtectionAsync(PdfDocument document, string currentPassword)
        {
            return await Task.Run(() =>
            {
                if (document == null || !document.Security.IsEncrypted)
                    return true;

                // Verify password
                if (document.Security.Password != currentPassword &&
                    document.Security.OwnerPassword != currentPassword)
                    return false;

                document.Security.EncryptionType = EncryptionType.None;
                document.Security.Password = string.Empty;
                document.Security.OwnerPassword = string.Empty;
                document.IsModified = true;

                return true;
            });
        }

        public async Task SetDocumentPermissionsAsync(PdfDocument document, Permissions permissions)
        {
            await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));

                document.Security.AllowPrinting = permissions.CanPrint;
                document.Security.AllowModification = permissions.CanModify;
                document.Security.AllowCopyContent = permissions.CanCopy;
                document.Security.AllowAnnotations = permissions.CanAddAnnotations;
                document.Security.AllowFormFilling = permissions.CanFillForms;
                document.Security.AllowAccessibility = permissions.CanExtractContent;
                document.Security.AllowAssembly = permissions.CanAssemble;
                document.IsModified = true;
            });
        }

        public async Task<Permissions> GetDocumentPermissionsAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null) return new Permissions();

                return new Permissions
                {
                    CanPrint = document.Security.AllowPrinting,
                    CanModify = document.Security.AllowModification,
                    CanCopy = document.Security.AllowCopyContent,
                    CanAddAnnotations = document.Security.AllowAnnotations,
                    CanFillForms = document.Security.AllowFormFilling,
                    CanExtractContent = document.Security.AllowAccessibility,
                    CanAssemble = document.Security.AllowAssembly,
                    CanPrintHighQuality = document.Security.AllowPrinting // Simplified
                };
            });
        }

        public async Task<bool> HasPermissionAsync(PdfDocument document, PermissionType permissionType)
        {
            return await Task.Run(() =>
            {
                if (document == null) return false;

                return permissionType switch
                {
                    PermissionType.Print => document.Security.AllowPrinting,
                    PermissionType.Modify => document.Security.AllowModification,
                    PermissionType.Copy => document.Security.AllowCopyContent,
                    PermissionType.Annotate => document.Security.AllowAnnotations,
                    PermissionType.FillForms => document.Security.AllowFormFilling,
                    PermissionType.ExtractContent => document.Security.AllowAccessibility,
                    PermissionType.Assemble => document.Security.AllowAssembly,
                    _ => true
                };
            });
        }

        public async Task<bool> CanPrintDocumentAsync(PdfDocument document)
        {
            return await HasPermissionAsync(document, PermissionType.Print);
        }

        public async Task<bool> CanModifyDocumentAsync(PdfDocument document)
        {
            return await HasPermissionAsync(document, PermissionType.Modify);
        }

        public async Task<bool> CanCopyContentAsync(PdfDocument document)
        {
            return await HasPermissionAsync(document, PermissionType.Copy);
        }

        public async Task<SecurityAuditResult> AuditDocumentSecurityAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null)
                    return new SecurityAuditResult { SecurityWarnings = { "Document is null" } };

                var result = new SecurityAuditResult
                {
                    IsEncrypted = document.Security.IsEncrypted,
                    EncryptionType = document.Security.EncryptionType,
                    KeyLength = document.Security.KeyLength,
                    EffectivePermissions = GetDocumentPermissionsAsync(document).Result
                };

                // Check password strength
                if (document.Security.IsEncrypted)
                {
                    result.HasStrongPassword = ValidatePasswordStrengthAsync(document.Security.Password).Result;
                    if (!result.HasStrongPassword)
                    {
                        result.SecurityWarnings.Add("Weak password detected");
                    }
                }

                // Check for excessive permissions
                var permissions = result.EffectivePermissions;
                if (permissions.CanModify && document.Security.IsEncrypted)
                {
                    result.SecurityWarnings.Add("Modification allowed on encrypted document");
                }

                if (permissions.CanAssemble && !document.Security.IsEncrypted)
                {
                    result.SecurityWarnings.Add("Document assembly allowed without encryption");
                }

                // Recommendations
                if (!document.Security.IsEncrypted)
                {
                    result.SecurityRecommendations.Add("Consider encrypting the document for better security");
                }

                if (document.Security.IsEncrypted && !result.HasStrongPassword)
                {
                    result.SecurityRecommendations.Add("Use a stronger password with mixed case, numbers, and symbols");
                }

                return result;
            });
        }

        public async Task<bool> IsDocumentEncryptedAsync(PdfDocument document)
        {
            return await Task.Run(() => document?.Security?.IsEncrypted ?? false);
        }

        public async Task<int> GetEncryptionLevelAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null || !document.Security.IsEncrypted)
                    return 0;

                return document.Security.EncryptionType switch
                {
                    EncryptionType.AES256 => 256,
                    EncryptionType.RC4 => 128,
                    _ => 0
                };
            });
        }

        public async Task<bool> AddDigitalSignatureAsync(PdfDocument document, byte[] certificateData, string password)
        {
            return await Task.Run(() =>
            {
                if (document == null || certificateData == null || certificateData.Length == 0)
                    return false;

                // PDFSharp digital signature implementation would go here
                // This is a placeholder for the actual implementation

                document.IsModified = true;
                return true;
            });
        }

        public async Task<bool> VerifyDigitalSignatureAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null)
                    return false;

                // PDFSharp signature verification would go here
                // This is a placeholder
                return false;
            });
        }

        // Helper methods for encryption/decryption
        private byte[] EncryptAes256(string plainText, string password)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;

                // Generate salt and IV
                var salt = GenerateRandomBytes(16);
                var iv = GenerateRandomBytes(16);

                // Derive key from password
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    aes.Key = deriveBytes.GetBytes(32); // 256 bits
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var ms = new System.IO.MemoryStream())
                    {
                        ms.Write(salt, 0, salt.Length);
                        ms.Write(iv, 0, iv.Length);

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new System.IO.StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        return ms.ToArray();
                    }
                }
            }
        }

        private string DecryptAes256(byte[] cipherText, string password)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    using (var ms = new System.IO.MemoryStream(cipherText))
                    {
                        // Read salt and IV
                        var salt = new byte[16];
                        var iv = new byte[16];
                        ms.Read(salt, 0, salt.Length);
                        ms.Read(iv, 0, iv.Length);

                        // Derive key from password
                        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                        {
                            aes.Key = deriveBytes.GetBytes(32);
                            aes.IV = iv;

                            using (var decryptor = aes.CreateDecryptor())
                            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            using (var sr = new System.IO.StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return null; // Decryption failed
            }
        }

        private byte[] GenerateRandomBytes(int length)
        {
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}