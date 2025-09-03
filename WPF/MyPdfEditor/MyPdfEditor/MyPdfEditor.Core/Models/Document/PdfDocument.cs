// MyPdfEditor.Core/Models/Document/PdfDocument.cs
using System.Collections.Generic;
using System.IO;
using MyPdfEditor.Core.Models.Security;

namespace MyPdfEditor.Core.Models.Document
{
    public class PdfDocument
    {
        public string FilePath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = "New Document";
        public PdfMetadata Metadata { get; set; } = new PdfMetadata();
        public List<PdfPage> Pages { get; set; } = new List<PdfPage>();
        public SecurityOptions Security { get; set; } = new SecurityOptions();
        public bool IsModified { get; set; }

        public PdfDocument()
        {
            // Add initial page
            AddPage();
        }

        public void AddPage()
        {
            Pages.Add(new PdfPage(Pages.Count + 1));
        }

        public void InsertPage(int index)
        {
            if (index >= 0 && index <= Pages.Count)
            {
                Pages.Insert(index, new PdfPage(index + 1));
                RenumberPages();
            }
        }

        public void RemovePage(int index)
        {
            if (index >= 0 && index < Pages.Count)
            {
                Pages.RemoveAt(index);
                RenumberPages();
            }
        }

        public void MovePage(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < Pages.Count &&
                toIndex >= 0 && toIndex < Pages.Count)
            {
                var page = Pages[fromIndex];
                Pages.RemoveAt(fromIndex);
                Pages.Insert(toIndex, page);
                RenumberPages();
            }
        }

        private void RenumberPages()
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].PageNumber = i + 1;
            }
        }

        public string GetFileName()
        {
            return !string.IsNullOrEmpty(FilePath) ?
                Path.GetFileName(FilePath) :
                $"{DocumentName}.pdf";
        }
    }
}