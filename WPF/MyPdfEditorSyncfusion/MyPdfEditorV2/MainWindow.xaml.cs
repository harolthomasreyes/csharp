using System.IO;
using System.Windows;
using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MyPdfEditorV2
{
    public partial class MainWindow : Window
    {
        private PdfDocument? currentDocument;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Create a new PDF document
        public void CreateNewPdf()
        {
            currentDocument = new PdfDocument();
            currentDocument.Info.Title = "New PDF Document";
            MessageBox.Show("New PDF document created.");
        }

        // Load an existing PDF document from file
        public void LoadPdfFromFile(string filePath)
        {
            currentDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Modify);
            DisplayPdf(filePath);
            MessageBox.Show("PDF document loaded.");
        }

        // Save the PDF document (overwrite or new file)
        public void SavePdfToFile(string filePath)
        {
            currentDocument?.Save(filePath);
            DisplayPdf(filePath);
            MessageBox.Show("PDF document saved.");
        }

        // Import PDF from bytes
        public void ImportPdfFromBytes(byte[] pdfBytes)
        {
            using var stream = new MemoryStream(pdfBytes);
            currentDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Modify);
            MessageBox.Show("PDF document imported from bytes.");
        }

        // Export PDF to bytes
        public byte[]? ExportPdfToBytes()
        {
            if (currentDocument == null) return null;
            using var stream = new MemoryStream();
            currentDocument.Save(stream, false);
            MessageBox.Show("PDF document exported to bytes.");
            return stream.ToArray();
        }

        // Validate PDF document structure
        public bool ValidatePdfStructure(string filePath)
        {
            try
            {
                using var doc = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly);
                return doc.PageCount > 0;
            }
            catch
            {
                return false;
            }
        }

        // Merge multiple PDF documents
        public void MergePdfs(string[] filePaths)
        {
            var mergedDocument = new PdfDocument();
            foreach (var path in filePaths)
            {
                using var inputDoc = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                foreach (var page in inputDoc.Pages)
                {
                    mergedDocument.AddPage(page);
                }
            }
            currentDocument = mergedDocument;
            // Display the merged PDF
            if (currentDocument != null && currentDocument.PageCount > 0)
            {
                // Save to a temporary file and display
                var tempFilePath = Path.Combine(Path.GetTempPath(), "merged.pdf");
                mergedDocument.Save(tempFilePath);
                DisplayPdf(tempFilePath);
            }
        }

        // Split PDF document by pages
        public void SplitPdfByPages(string filePath, int[] pageNumbers, string outputDirectory)
        {
            using var inputDoc = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            foreach (var pageNum in pageNumbers)
            {
                var splitDoc = new PdfDocument();
                splitDoc.AddPage(inputDoc.Pages[pageNum]);
                var outputPath = Path.Combine(outputDirectory, $"Split_Page_{pageNum + 1}.pdf");
                splitDoc.Save(outputPath);
            }
            MessageBox.Show("PDF document split by pages.");
        }

        // Display PDF in the viewer control
        private void DisplayPdf(string filePath)
        {
            if (File.Exists(filePath))
            {
                PdfViewerControl.Load(filePath);
            }
        }

        // Event handlers for buttons
        private void BtnCreatePdf_Click(object sender, RoutedEventArgs e)
        {
            CreateNewPdf();
        }

        private void BtnLoadPdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (openFileDialog.ShowDialog() == true)
            {
                LoadPdfFromFile(openFileDialog.FileName);
            }
        }

        private void BtnSavePdf_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                SavePdfToFile(saveFileDialog.FileName);
            }
        }

        private void BtnImportPdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (openFileDialog.ShowDialog() == true)
            {
                var bytes = File.ReadAllBytes(openFileDialog.FileName);
                ImportPdfFromBytes(bytes);
            }
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (saveFileDialog.ShowDialog() == true && currentDocument != null)
            {
                var bytes = ExportPdfToBytes();
                if (bytes != null)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, bytes);
                }
            }
        }

        private void BtnValidatePdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (openFileDialog.ShowDialog() == true)
            {
                ValidatePdfStructure(openFileDialog.FileName);
            }
        }

        private void BtnMergePdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                MergePdfs(openFileDialog.FileNames);
            }
        }

        private void BtnSplitPdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (openFileDialog.ShowDialog() == true)
            {
                var inputFile = openFileDialog.FileName;
                var outputDir = Path.GetDirectoryName(inputFile) ?? "";
                // Example: split first 3 pages
                SplitPdfByPages(inputFile, new int[] { 0, 1, 2 }, outputDir);
            }
        }
    }
}