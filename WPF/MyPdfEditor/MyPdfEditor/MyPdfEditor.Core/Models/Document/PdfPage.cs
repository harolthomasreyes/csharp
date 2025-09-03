


using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.Core.Models.Document
{
    public class PdfPage
    {
        public int PageNumber { get; set; }
        public double Width { get; set; } = 595; // A4 width in points (210mm)
        public double Height { get; set; } = 842; // A4 height in points (297mm)
        public List<FormField> FormFields { get; set; } = new List<FormField>();

        public PdfPage(int pageNumber)
        {
            PageNumber = pageNumber;
        }

        public void AddField(FormField field)
        {
            FormFields.Add(field);
        }

        public void RemoveField(FormField field)
        {
            FormFields.Remove(field);
        }

        public IEnumerable<FormField> GetFields()
        {
            return FormFields.AsReadOnly();
        }
    }
}