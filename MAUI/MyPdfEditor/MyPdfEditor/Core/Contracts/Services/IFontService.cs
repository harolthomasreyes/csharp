namespace MyPdfEditor.Core.Contracts.Services
{
    public interface IFontService
    {
        Task<IEnumerable<FontInfo>> GetAvailableFontsAsync();
        Task<FontInfo> GetFontInfoAsync(string fontName);
        Task<bool> InstallFontAsync(byte[] fontData, string fontName);
        Task<bool> UninstallFontAsync(string fontName);
        Task<byte[]> GetFontBytesAsync(string fontName);
        Task<bool> ValidateFontCompatibilityAsync(string fontName);
    }

    public class FontInfo
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsInstalled { get; set; }
    }
}
