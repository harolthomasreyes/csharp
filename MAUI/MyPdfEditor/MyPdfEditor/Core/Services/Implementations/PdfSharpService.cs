// MyPdfEditor.Core/Services/Implementations/PdfSharpService.cs
using MyPdfEditor.Core.Contracts.Services;
using MyPdfEditor.Core.Models;
using MyPdfEditor.Core.Models.Enums;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class PdfSharpService : IPdfService
    {
        public async Task<PdfDocument> CreateNewDocumentAsync()
        {
            return await Task.Run(() =>
            {
                var document = new PdfDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "New Document",
                    PageCount = 1,
                    Status = DocumentStatus.New,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                document.Pages.Add(new PdfPage
                {
                    PageNumber = 1,
                    Width = 595, // A4 width in points
                    Height = 842, // A4 height in points
                    Orientation = PageOrientation.Portrait
                });

                return document;
            });
        }

        public async Task<PdfDocument> LoadDocumentAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("PDF file not found", filePath);

                using var pdfDocument = PdfSharp.Pdf.IO.PdfReader.Open(filePath, PdfDocumentOpenMode.Modify);

                var document = new PdfDocument
                {
                    FilePath = filePath,
                    Title = Path.GetFileNameWithoutExtension(filePath),
                    PageCount = pdfDocument.PageCount,
                    Status = DocumentStatus.Loaded,
                    CreatedDate = File.GetCreationTime(filePath),
                    ModifiedDate = File.GetLastWriteTime(filePath)
                };

                // Load pages
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    var pdfPage = pdfDocument.Pages[i];
                    document.Pages.Add(new PdfPage
                    {
                        PageNumber = i + 1,
                        Width = pdfPage.Width.Point,
                        Height = pdfPage.Height.Point,
                        Orientation = pdfPage.Width > pdfPage.Height ? PageOrientation.Landscape : PageOrientation.Portrait
                    });
                }

                // Load form elements
                if (pdfDocument.AcroForms != null)
                {
                    LoadFormElements(pdfDocument, document);
                }

                return document;
            });
        }

        public async Task SaveDocumentAsync(PdfDocument document, string filePath)
        {
            await Task.Run(() =>
            {
                using var pdfDocument = new PdfSharp.Pdf.PdfDocument();

                // Create pages
                foreach (var page in document.Pages)
                {
                    var pdfPage = pdfDocument.AddPage();
                    pdfPage.Width = page.Width;
                    pdfPage.Height = page.Height;
                }

                // Add form elements
                if (document.FormElements.Count > 0)
                {
                    CreateFormElements(pdfDocument, document);
                }

                pdfDocument.Save(filePath);
                document.FilePath = filePath;
                document.Status = DocumentStatus.Saved;
                document.ModifiedDate = DateTime.Now;
            });
        }

        public async Task<PdfDocument> ImportDocumentAsync(byte[] fileData)
        {
            return await Task.Run(() =>
            {
                using var stream = new MemoryStream(fileData);
                using var pdfDocument = PdfSharp.Pdf.IO.PdfReader.Open(stream, PdfDocumentOpenMode.Import);

                var document = new PdfDocument
                {
                    Title = "Imported Document",
                    PageCount = pdfDocument.PageCount,
                    Status = DocumentStatus.Loaded,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Import pages and elements similar to LoadDocumentAsync
                return document;
            });
        }

        public async Task<byte[]> ExportDocumentAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                using var pdfDocument = new PdfSharp.Pdf.PdfDocument();
                using var stream = new MemoryStream();

                // Recreate document similar to SaveDocumentAsync
                pdfDocument.Save(stream);
                return stream.ToArray();
            });
        }

        public async Task<bool> ValidateDocumentAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null) return false;
                if (document.PageCount <= 0) return false;

                foreach (var page in document.Pages)
                {
                    if (page.Width <= 0 || page.Height <= 0) return false;
                }

                return true;
            });
        }

        public async Task<PdfDocument> MergeDocumentsAsync(IEnumerable<PdfDocument> documents)
        {
            return await Task.Run(() =>
            {
                var mergedDocument = new PdfDocument
                {
                    Title = "Merged Document",
                    Status = DocumentStatus.New,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                int pageOffset = 0;
                foreach (var doc in documents)
                {
                    foreach (var page in doc.Pages)
                    {
                        mergedDocument.Pages.Add(new PdfPage
                        {
                            PageNumber = mergedDocument.Pages.Count + 1,
                            Width = page.Width,
                            Height = page.Height,
                            Orientation = page.Orientation
                        });
                    }

                    // Merge form elements with adjusted positions
                    foreach (var element in doc.FormElements)
                    {
                        var mergedElement = element.Clone();
                        mergedElement.Y += pageOffset * mergedDocument.Pages[0].Height;
                        mergedDocument.FormElements.Add(mergedElement);
                    }

                    pageOffset += doc.PageCount;
                }

                mergedDocument.PageCount = mergedDocument.Pages.Count;
                return mergedDocument;
            });
        }

        public async Task<PdfDocument> SplitDocumentAsync(PdfDocument document, int startPage, int endPage)
        {
            return await Task.Run(() =>
            {
                if (startPage < 1 || endPage > document.PageCount || startPage > endPage)
                    throw new ArgumentException("Invalid page range");

                var splitDocument = new PdfDocument
                {
                    Title = $"{document.Title} (Pages {startPage}-{endPage})",
                    Status = DocumentStatus.New,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Add selected pages
                for (int i = startPage - 1; i < endPage; i++)
                {
                    var originalPage = document.Pages[i];
                    splitDocument.Pages.Add(new PdfPage
                    {
                        PageNumber = splitDocument.Pages.Count + 1,
                        Width = originalPage.Width,
                        Height = originalPage.Height,
                        Orientation = originalPage.Orientation
                    });
                }

                // Add form elements that belong to the selected pages
                foreach (var element in document.FormElements)
                {
                    if (element.PageNumber >= startPage && element.PageNumber <= endPage)
                    {
                        var splitElement = element.Clone();
                        splitElement.PageNumber -= (startPage - 1);
                        splitDocument.FormElements.Add(splitElement);
                    }
                }

                splitDocument.PageCount = splitDocument.Pages.Count;
                return splitDocument;
            });
        }

        private void LoadFormElements(PdfSharp.Pdf.PdfDocument pdfDocument, PdfDocument document)
        {
            if (pdfDocument.AcroForms?.Fields == null) return;

            foreach (var field in pdfDocument.AcroForms.Fields)
            {
                var element = CreatePdfElementFromField(field);
                if (element != null)
                {
                    document.FormElements.Add(element);
                }
            }
        }

        private PdfElement CreatePdfElementFromField(PdfAcroField field)
        {
            return field switch
            {
                PdfTextField textField => new PdfElement
                {
                    Id = field.Name,
                    Type = PdfElementType.TextField,
                    X = textField.Rectangle.X1,
                    Y = textField.Rectangle.Y1,
                    Width = textField.Rectangle.Width,
                    Height = textField.Rectangle.Height,
                    Text = textField.Value?.ToString(),
                    PageNumber = GetPageNumber(field)
                },
                PdfCheckBox checkBox => new PdfElement
                {
                    Id = field.Name,
                    Type = PdfElementType.Checkbox,
                    X = checkBox.Rectangle.X1,
                    Y = checkBox.Rectangle.Y1,
                    Width = checkBox.Rectangle.Width,
                    Height = checkBox.Rectangle.Height,
                    IsChecked = checkBox.Checked,
                    PageNumber = GetPageNumber(field)
                },
                PdfRadioButton radioButton => new PdfElement
                {
                    Id = field.Name,
                    Type = PdfElementType.RadioButton,
                    X = radioButton.Rectangle.X1,
                    Y = radioButton.Rectangle.Y1,
                    Width = radioButton.Rectangle.Width,
                    Height = radioButton.Rectangle.Height,
                    IsChecked = radioButton.Checked,
                    PageNumber = GetPageNumber(field)
                },
                _ => null
            };
        }

        private int GetPageNumber(PdfAcroField field)
        {
            // Implementation to get page number from field
            return 1; // Simplified for example
        }

        private void CreateFormElements(PdfSharp.Pdf.PdfDocument pdfDocument, PdfDocument document)
        {
            // Implementation to create PDFsharp form elements from our model
        }
    }
}