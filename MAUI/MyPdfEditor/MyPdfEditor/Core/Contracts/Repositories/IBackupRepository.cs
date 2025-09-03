using PdfSharp.Pdf;

namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface IBackupRepository
    {
        Task CreateBackupAsync(PdfDocument document, string backupName);
        Task<PdfDocument> RestoreBackupAsync(string backupName);
        Task<IEnumerable<string>> GetAvailableBackupsAsync();
        Task DeleteBackupAsync(string backupName);
        Task<bool> BackupExistsAsync(string backupName);
        Task AutoBackupAsync(PdfDocument document);
        Task CleanOldBackupsAsync(int daysToKeep);
    }
}
