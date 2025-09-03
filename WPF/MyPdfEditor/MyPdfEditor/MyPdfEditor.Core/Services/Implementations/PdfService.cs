// MyPdfEditor.Core/Services/Implementations/PdfService.cs
using System.IO;
using PdfSharp.Pdf.IO;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Services.Interfaces;
using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class PdfService : IPdfService
    {
        public async Task<PdfDocument> CreateNewDocumentAsync(double width, double height)
        {
            return await Task.Run(() =>
            {
                var document = new PdfDocument();
                document.Metadata = new PdfMetadata
                {
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now
                };

                var page = new PdfPage(1)
                {
                    Width = width,
                    Height = height
                };
                document.Pages.Add(page);

                return document;
            });
        }

        public async Task<PdfDocument> LoadDocumentAsync(string filePath, string password = null)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("PDF file not found", filePath);

                var sharpDoc = string.IsNullOrEmpty(password) ?
                    PdfReader.Open(filePath, PdfDocumentOpenMode.Modify) :
                    PdfReader.Open(filePath, password, PdfDocumentOpenMode.Modify);

                var document = new PdfDocument
                {
                    FilePath = filePath,
                    DocumentName = Path.GetFileNameWithoutExtension(filePath),
                    Metadata = ExtractMetadata(sharpDoc),
                    Security = ExtractSecurityInfo(sharpDoc)
                };

                // Convert PdfSharp pages to our model
                for (int i = 0; i < sharpDoc.PageCount; i++)
                {
                    var sharpPage = sharpDoc.Pages[i];
                    var page = new PdfPage(i + 1)
                    {
                        Width = sharpPage.Width.Point,
                        Height = sharpPage.Height.Point
                    };
                    document.Pages.Add(page);
                }

                return document;
            });
        }

        public async Task SaveDocumentAsync(PdfDocument document, string filePath, bool overwrite = true)
        {
            await Task.Run(() =>
            {
                if (File.Exists(filePath) && !overwrite)
                    throw new IOException("File already exists and overwrite is false");

                using (var sharpDoc = new PdfDocument())
                {
                    ApplyMetadata(sharpDoc, document.Metadata);
                    ApplySecurity(sharpDoc, document.Security);

                    foreach (var page in document.Pages)
                    {
                        var sharpPage = sharpDoc.AddPage();
                        sharpPage.Width = page.Width;
                        sharpPage.Height = page.Height;

                        // TODO: Add form fields to the PDF page
                    }

                    sharpDoc.Save(filePath);
                }

                document.FilePath = filePath;
                document.IsModified = false;
            });
        }

        public async Task<byte[]> ExportToBytesAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                using (var sharpDoc = new PdfDocument())
                using (var stream = new MemoryStream())
                {
                    ApplyMetadata(sharpDoc, document.Metadata);
                    ApplySecurity(sharpDoc, document.Security);

                    foreach (var page in document.Pages)
                    {
                        var sharpPage = sharpDoc.AddPage();
                        sharpPage.Width = page.Width;
                        sharpPage.Height = page.Height;
                    }

                    sharpDoc.Save(stream, false);
                    return stream.ToArray();
                }
            });
        }

        public async Task<PdfDocument> ImportFromBytesAsync(byte[] pdfData, string password = null)
        {
            return await Task.Run(() =>
            {
                using (var stream = new MemoryStream(pdfData))
                {
                    var sharpDoc = string.IsNullOrEmpty(password) ?
                        PdfReader.Open(stream, PdfDocumentOpenMode.Modify) :
                        PdfReader.Open(stream, password, PdfDocumentOpenMode.Modify);

                    var document = new PdfDocument
                    {
                        Metadata = ExtractMetadata(sharpDoc),
                        Security = ExtractSecurityInfo(sharpDoc)
                    };

                    for (int i = 0; i < sharpDoc.PageCount; i++)
                    {
                        var sharpPage = sharpDoc.Pages[i];
                        var page = new PdfPage(i + 1)
                        {
                            Width = sharpPage.Width.Point,
                            Height = sharpPage.Height.Point
                        };
                        document.Pages.Add(page);
                    }

                    return document;
                }
            });
        }

        public async Task<bool> ValidateDocumentStructureAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null) return false;
                if (document.Pages == null || document.Pages.Count == 0) return false;

                foreach (var page in document.Pages)
                {
                    if (page.Width <= 0 || page.Height <= 0) return false;
                    if (page.FormFields == null) return false;
                }

                return true;
            });
        }

        public async Task<bool> IsValidPdfFileAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var doc = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
                    {
                        return doc.PageCount > 0;
                    }
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<PdfDocument> MergeDocumentsAsync(IEnumerable<PdfDocument> documents)
        {
            return await Task.Run(() =>
            {
                var mergedDoc = new PdfDocument { DocumentName = "Merged Document" };

                foreach (var doc in documents)
                {
                    foreach (var page in doc.Pages)
                    {
                        mergedDoc.Pages.Add(page);
                    }
                }

                return mergedDoc;
            });
        }

        public async Task<PdfDocument> MergeDocumentsFromPathsAsync(IEnumerable<string> filePaths)
        {
            var documents = new List<PdfDocument>();

            foreach (var path in filePaths)
            {
                var doc = await LoadDocumentAsync(path);
                documents.Add(doc);
            }

            return await MergeDocumentsAsync(documents);
        }

        public async Task<IEnumerable<PdfDocument>> SplitDocumentByPagesAsync(PdfDocument document, IEnumerable<int> pageNumbers)
        {
            return await Task.Run(() =>
            {
                var result = new List<PdfDocument>();
                var pagesList = pageNumbers.ToList();

                foreach (var pageNum in pagesList)
                {
                    if (pageNum > 0 && pageNum <= document.Pages.Count)
                    {
                        var newDoc = new PdfDocument
                        {
                            DocumentName = $"{document.DocumentName}_Page{pageNum}"
                        };
                        newDoc.Pages.Add(document.Pages[pageNum - 1]);
                        result.Add(newDoc);
                    }
                }

                return result;
            });
        }

        public async Task<IEnumerable<PdfDocument>> SplitDocumentByRangesAsync(PdfDocument document, IEnumerable<(int Start, int End)> ranges)
        {
            return await Task.Run(() =>
            {
                var result = new List<PdfDocument>();

                foreach (var range in ranges)
                {
                    if (range.Start > 0 && range.End <= document.Pages.Count && range.Start <= range.End)
                    {
                        var newDoc = new PdfDocument
                        {
                            DocumentName = $"{document.DocumentName}_Pages{range.Start}-{range.End}"
                        };

                        for (int i = range.Start - 1; i < range.End; i++)
                        {
                            newDoc.Pages.Add(document.Pages[i]);
                        }

                        result.Add(newDoc);
                    }
                }

                return result;
            });
        }

        public async Task AddPageAsync(PdfDocument document, double width, double height)
        {
            await Task.Run(() =>
            {
                document.AddPage();
                var lastPage = document.Pages[^1];
                lastPage.Width = width;
                lastPage.Height = height;
            });
        }

        public async Task RemovePageAsync(PdfDocument document, int pageNumber)
        {
            await Task.Run(() => document.RemovePage(pageNumber - 1));
        }

        public async Task ReorderPagesAsync(PdfDocument document, IEnumerable<int> newOrder)
        {
            await Task.Run(() =>
            {
                var orderedPages = newOrder
                    .Where(n => n > 0 && n <= document.Pages.Count)
                    .Select(n => document.Pages[n - 1])
                    .ToList();

                document.Pages = orderedPages;
                document.RenumberPages();
            });
        }

        public async Task EncryptDocumentAsync(PdfDocument document, string password, EncryptionType encryptionType)
        {
            await Task.Run(() =>
            {
                document.Security.EncryptionType = encryptionType;
                document.Security.Password = password;
                document.Security.OwnerPassword = password;
                document.IsModified = true;
            });
        }

        public async Task DecryptDocumentAsync(PdfDocument document, string password)
        {
            await Task.Run(() =>
            {
                // For now, just clear encryption settings
                // Actual decryption would require PDFSharp operations
                document.Security.EncryptionType = EncryptionType.None;
                document.Security.Password = string.Empty;
                document.Security.OwnerPassword = string.Empty;
                document.IsModified = true;
            });
        }

        public async Task<bool> ValidatePasswordAsync(PdfDocument document, string password)
        {
            return await Task.Run(() =>
            {
                if (document.Security.EncryptionType == EncryptionType.None)
                    return true;

                return document.Security.Password == password ||
                       document.Security.OwnerPassword == password;
            });
        }

        private PdfMetadata ExtractMetadata(PdfDocument sharpDoc)
        {
            return new PdfMetadata
            {
                Title = sharpDoc.Info.Title,
                Author = sharpDoc.Info.Author,
                Subject = sharpDoc.Info.Subject,
                Keywords = sharpDoc.Info.Keywords,
                Creator = sharpDoc.Info.Creator,
                CreationDate = sharpDoc.Info.CreationDate,
                ModificationDate = sharpDoc.Info.ModificationDate,
                PageCount = sharpDoc.PageCount
            };
        }

        private SecurityOptions ExtractSecurityInfo(PdfDocument sharpDoc)
        {
            return new SecurityOptions
            {
                // PDFSharp doesn't expose security info directly in this way
                // This would need to be implemented with more advanced PDF manipulation
                EncryptionType = EncryptionType.None
            };
        }

        private void ApplyMetadata(PdfDocument sharpDoc, PdfMetadata metadata)
        {
            sharpDoc.Info.Title = metadata.Title;
            sharpDoc.Info.Author = metadata.Author;
            sharpDoc.Info.Subject = metadata.Subject;
            sharpDoc.Info.Keywords = metadata.Keywords;
            sharpDoc.Info.Creator = metadata.Creator;
            sharpDoc.Info.CreationDate = metadata.CreationDate;
            sharpDoc.Info.ModificationDate = metadata.ModificationDate;
        }

        private void ApplySecurity(PdfDocument sharpDoc, SecurityOptions security)
        {
            if (security.IsEncrypted)
            {
                // PDFSharp security implementation would go here
                // This is simplified for the example
            }
        }
    }
}