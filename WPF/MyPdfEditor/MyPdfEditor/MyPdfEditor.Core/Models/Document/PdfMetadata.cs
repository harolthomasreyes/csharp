namespace MyPdfEditor.Core.Models.Document
{
    public class PdfMetadata
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string Creator { get; set; } = "MyPdfEditor";
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModificationDate { get; set; } = DateTime.Now;
        public int PageCount { get; set; }
        public string Version { get; set; } = "1.0";
    }
}