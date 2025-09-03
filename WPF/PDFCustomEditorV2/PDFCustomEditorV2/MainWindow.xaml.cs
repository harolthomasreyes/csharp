using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using PdfiumViewer;
using Microsoft.Win32;
using System.Windows.Media;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Windows.Input;
using System.Globalization;

namespace PDFCustomEditorV2
{
    public partial class MainWindow : Window
    {
        // Documento para visualización (PdfiumViewer)
        private PdfDocument pdfViewerDocument;

        // Documento para edición (PDFsharp) - se crea temporalmente al guardar
        private string currentPdfPath;
        private List<EditableTextBox> textAnnotations = new List<EditableTextBox>();
        private int currentPage = 0;

        // Clase para almacenar información de los textboxes editables
        public class EditableTextBox
        {
            public System.Windows.Controls.TextBox TextBox { get; set; }
            public int PageNumber { get; set; }
            public System.Windows.Point Position { get; set; }
            public string FontName { get; set; }
            public double FontSize { get; set; }
            public System.Windows.Media.Color FontColor { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            pdfViewer.PageChanged += PdfViewer_PageChanged;
        }

        private void btnLoadPdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                Title = "Seleccionar PDF"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadPdf(openFileDialog.FileName);
            }
        }

        private void LoadPdf(string filePath)
        {
            try
            {
                ClearOverlay();
                textAnnotations.Clear();
                currentPdfPath = filePath;

                // Liberar documento anterior si existe
                pdfViewerDocument?.Dispose();

                // Cargar el PDF para visualización (PdfiumViewer)
                pdfViewerDocument = PdfDocument.Load(filePath);
                pdfViewer.Document = pdfViewerDocument;
                pdfViewer.ZoomMode = PdfViewerZoomMode.FitWidth;

                UpdateStatus($"PDF cargado: {Path.GetFileName(filePath)}");
                UpdatePageInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el PDF: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddTextBox_Click(object sender, RoutedEventArgs e)
        {
            if (pdfViewerDocument == null)
            {
                MessageBox.Show("Primero carga un PDF", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddTextBox();
        }

        private void AddTextBox()
        {
            // Obtener configuración de fuente
            double fontSize = 12;
            if (cmbFontSize.SelectedItem is ComboBoxItem fontSizeItem &&
                double.TryParse(fontSizeItem.Content.ToString(), out double size))
            {
                fontSize = size;
            }

            // Obtener color
            System.Windows.Media.Color fontColor = Colors.Black;
            if (cmbFontColor.SelectedItem is ComboBoxItem colorItem &&
                colorItem.Tag is string colorTag)
            {
                fontColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(colorTag);
            }

            var textBox = new System.Windows.Controls.TextBox
            {
                Width = 200,
                Height = 30,
                Background = new SolidColorBrush(Colors.LightYellow) { Opacity = 0.8 },
                Foreground = new SolidColorBrush(fontColor),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                FontSize = fontSize,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                ToolTip = "Haz doble clic para eliminar"
            };

            // Posicionar en el centro
            double left = (pdfHost.ActualWidth - textBox.Width) / 2;
            double top = (pdfHost.ActualHeight - textBox.Height) / 2;

            Canvas.SetLeft(textBox, left);
            Canvas.SetTop(textBox, top);

            // Hacer el TextBox arrastrable
            textBox.MouseDown += TextBox_MouseDown;
            textBox.MouseMove += TextBox_MouseMove;
            textBox.MouseUp += TextBox_MouseUp;
            textBox.MouseDoubleClick += TextBox_MouseDoubleClick;

            overlayCanvas.Children.Add(textBox);

            // Guardar la anotación
            var annotation = new EditableTextBox
            {
                TextBox = textBox,
                PageNumber = currentPage,
                Position = new System.Windows.Point(left, top),
                FontSize = fontSize,
                FontColor = fontColor,
                FontName = "Arial"
            };

            textAnnotations.Add(annotation);
            textBox.Focus();
        }

        private bool isDragging = false;
        private System.Windows.Point dragStartPoint;
        private System.Windows.Controls.TextBox draggedTextBox;

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                isDragging = true;
                draggedTextBox = textBox;
                dragStartPoint = e.GetPosition(overlayCanvas);
                textBox.CaptureMouse();
            }
        }

        private void TextBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging && draggedTextBox != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = e.GetPosition(overlayCanvas);
                var offset = currentPoint - dragStartPoint;

                Canvas.SetLeft(draggedTextBox, Canvas.GetLeft(draggedTextBox) + offset.X);
                Canvas.SetTop(draggedTextBox, Canvas.GetTop(draggedTextBox) + offset.Y);

                dragStartPoint = currentPoint;

                // Actualizar la posición en la anotación
                var annotation = textAnnotations.Find(a => a.TextBox == draggedTextBox);
                if (annotation != null)
                {
                    annotation.Position = new System.Windows.Point(
                        Canvas.GetLeft(draggedTextBox),
                        Canvas.GetTop(draggedTextBox));
                }
            }
        }

        private void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            draggedTextBox?.ReleaseMouseCapture();
            draggedTextBox = null;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                overlayCanvas.Children.Remove(textBox);
                textAnnotations.RemoveAll(a => a.TextBox == textBox);
            }
        }

        private void btnSavePdf_Click(object sender, RoutedEventArgs e)
        {
            if (pdfViewerDocument == null)
            {
                MessageBox.Show("Primero carga un PDF", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SavePdfWithAnnotations();
        }

        private void SavePdfWithAnnotations()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                Title = "Guardar PDF con anotaciones",
                FileName = Path.GetFileNameWithoutExtension(currentPdfPath) + "_anotado.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Cargar el PDF original con PDFsharp
                    using (var originalDoc = PdfSharp.Pdf.IO.PdfReader.Open(currentPdfPath, PdfDocumentOpenMode.Import))
                    {
                        // Crear nuevo documento PDFsharp
                        var newDoc = new PdfSharp.Pdf.PdfDocument();

                        // Copiar todas las páginas y agregar anotaciones
                        for (int i = 0; i < originalDoc.PageCount; i++)
                        {
                            var page = originalDoc.Pages[i];
                            var newPage = newDoc.AddPage(page);

                            // Obtener anotaciones para esta página
                            var pageAnnotations = textAnnotations.FindAll(a => a.PageNumber == i);

                            if (pageAnnotations.Count > 0)
                            {
                                // Crear graphics para dibujar en la página
                                using (XGraphics gfx = XGraphics.FromPdfPage(newPage))
                                {
                                    foreach (var annotation in pageAnnotations)
                                    {
                                        if (!string.IsNullOrEmpty(annotation.TextBox.Text))
                                        {
                                            // Convertir coordenadas de WPF a coordenadas de PDF
                                            var pdfPoint = ConvertToPdfCoordinates(annotation.Position, newPage);

                                            // Crear fuente y color
                                            XFont font = new XFont(annotation.FontName, annotation.FontSize);
                                            XColor color = XColor.FromArgb(
                                                annotation.FontColor.A,
                                                annotation.FontColor.R,
                                                annotation.FontColor.G,
                                                annotation.FontColor.B);

                                            // Dibujar texto
                                            gfx.DrawString(annotation.TextBox.Text, font,
                                                new XSolidBrush(color),
                                                new XPoint(pdfPoint.X, pdfPoint.Y));
                                        }
                                    }
                                }
                            }
                        }

                        // Guardar el documento
                        newDoc.Save(saveFileDialog.FileName);
                        newDoc.Close();
                    }

                    MessageBox.Show("PDF guardado exitosamente con las anotaciones!", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    UpdateStatus($"PDF guardado: {Path.GetFileName(saveFileDialog.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private System.Windows.Point ConvertToPdfCoordinates(System.Windows.Point wpfPoint, PdfPage page)
        {
            // Convertir coordenadas de WPF a coordenadas de PDF
            // Las coordenadas en PDF tienen (0,0) en la esquina inferior izquierda
            double scaleX = page.Width.Point / pdfHost.ActualWidth;
            double scaleY = page.Height.Point / pdfHost.ActualHeight;

            double pdfX = wpfPoint.X * scaleX;
            double pdfY = page.Height.Point - (wpfPoint.Y * scaleY);

            return new System.Windows.Point(pdfX, pdfY);
        }

        private void PdfViewer_PageChanged(object sender, EventArgs e)
        {
            currentPage = pdfViewer.Page;
            UpdatePageInfo();

            // Limpiar overlay y cargar anotaciones para la página actual
            ClearOverlay();
            LoadAnnotationsForCurrentPage();
        }

        private void LoadAnnotationsForCurrentPage()
        {
            var pageAnnotations = textAnnotations.FindAll(a => a.PageNumber == currentPage);

            foreach (var annotation in pageAnnotations)
            {
                // Clonar el TextBox para evitar problemas de referencia
                var textBox = new System.Windows.Controls.TextBox
                {
                    Text = annotation.TextBox.Text,
                    Width = annotation.TextBox.Width,
                    Height = annotation.TextBox.Height,
                    Background = annotation.TextBox.Background,
                    Foreground = annotation.TextBox.Foreground,
                    BorderBrush = annotation.TextBox.BorderBrush,
                    BorderThickness = annotation.TextBox.BorderThickness,
                    FontSize = annotation.TextBox.FontSize,
                    AcceptsReturn = annotation.TextBox.AcceptsReturn,
                    TextWrapping = annotation.TextBox.TextWrapping,
                    ToolTip = annotation.TextBox.ToolTip
                };

                textBox.MouseDown += TextBox_MouseDown;
                textBox.MouseMove += TextBox_MouseMove;
                textBox.MouseUp += TextBox_MouseUp;
                textBox.MouseDoubleClick += TextBox_MouseDoubleClick;

                overlayCanvas.Children.Add(textBox);
                Canvas.SetLeft(textBox, annotation.Position.X);
                Canvas.SetTop(textBox, annotation.Position.Y);

                // Actualizar referencia al TextBox real mostrado
                annotation.TextBox = textBox;
            }
        }

        private void ClearOverlay()
        {
            overlayCanvas.Children.Clear();
        }

        private void UpdateStatus(string message)
        {
            statusText.Text = message;
        }

        private void UpdatePageInfo()
        {
            if (pdfViewerDocument != null)
            {
                pageInfoText.Text = $"Página: {currentPage + 1}/{pdfViewerDocument.PageCount}";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            pdfViewerDocument?.Dispose();
            base.OnClosed(e);
        }

    }
}