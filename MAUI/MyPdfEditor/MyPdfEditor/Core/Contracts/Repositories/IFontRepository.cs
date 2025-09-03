namespace MyPdfEditor.Core.Contracts.Repositories
{
    public interface IFontRepository
    {
        Task<IEnumerable<string>> GetAvailableFontsAsync();
        Task<byte[]> GetFontDataAsync(string fontName);
        Task<bool> FontExistsAsync(string fontName);
        Task AddFontAsync(string fontName, byte[] fontData);
        Task RemoveFontAsync(string fontName);
    }
}
