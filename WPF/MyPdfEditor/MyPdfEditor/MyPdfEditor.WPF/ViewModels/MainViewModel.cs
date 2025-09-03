// MyPdfEditor.WPF/ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Enums;
using MyPdfEditor.Core.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IPdfService _pdfService;
        private readonly IUndoRedoService _undoRedoService;

        [ObservableProperty]
        private PdfDocument _currentDocument;

        [ObservableProperty]
        private DocumentViewModel _documentViewModel;

        [ObservableProperty]
        private bool _isDocumentOpen;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private double _zoomLevel = 100;

        [ObservableProperty]
        private bool _isBusy;

        public ObservableCollection<RecentDocument> RecentDocuments { get; } = new ObservableCollection<RecentDocument>();

        public ICommand NewDocumentCommand { get; }
        public ICommand OpenDocumentCommand { get; }
        public ICommand SaveDocumentCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand CloseDocumentCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand FitToWidthCommand { get; }
        public ICommand FitToHeightCommand { get; }
        public ICommand ActualSizeCommand { get; }

        public MainViewModel(IPdfService pdfService, IUndoRedoService undoRedoService)
        {
            _pdfService = pdfService;
            _undoRedoService = undoRedoService;

            NewDocumentCommand = new AsyncRelayCommand(NewDocumentAsync);
            OpenDocumentCommand = new AsyncRelayCommand(OpenDocumentAsync);
            SaveDocumentCommand = new AsyncRelayCommand(SaveDocumentAsync, CanSaveDocument);
            SaveAsCommand = new AsyncRelayCommand(SaveAsAsync, CanSaveDocument);
            CloseDocumentCommand = new RelayCommand(CloseDocument, CanCloseDocument);
            UndoCommand = new RelayCommand(Undo, () => _undoRedoService.CanUndo);
            RedoCommand = new RelayCommand(Redo, () => _undoRedoService.CanRedo);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            FitToWidthCommand = new RelayCommand(() => SetZoomMode(ZoomMode.FitWidth));
            FitToHeightCommand = new RelayCommand(() => SetZoomMode(ZoomMode.FitHeight));
            ActualSizeCommand = new RelayCommand(() => SetZoomMode(ZoomMode.ActualSize));

            _undoRedoService.StateChanged += OnUndoRedoStateChanged;
        }

        private async Task NewDocumentAsync()
        {
            try
            {
                IsBusy = true;
                var document = await _pdfService.CreateNewDocumentAsync(595, 842); // A4 size
                CurrentDocument = document;
                DocumentViewModel = new DocumentViewModel(document, _pdfService);
                IsDocumentOpen = true;
                StatusMessage = "New document created";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error creating document: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task OpenDocumentAsync()
        {
            // Implementation would use file dialog
            StatusMessage = "Open document functionality not implemented";
        }

        public async Task SaveDocumentAsync()
        {
            if (CurrentDocument == null) return;

            try
            {
                IsBusy = true;
                if (string.IsNullOrEmpty(CurrentDocument.FilePath))
                {
                    await SaveAsAsync();
                }
                else
                {
                    await _pdfService.SaveDocumentAsync(CurrentDocument, CurrentDocument.FilePath);
                    StatusMessage = "Document saved successfully";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving document: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsAsync()
        {
            // Implementation would use save file dialog
            StatusMessage = "Save As functionality not implemented";
        }

        private void CloseDocument()
        {
            CurrentDocument = null;
            DocumentViewModel = null;
            IsDocumentOpen = false;
            StatusMessage = "Document closed";
        }

        private bool CanSaveDocument() => IsDocumentOpen && CurrentDocument?.IsModified == true;
        private bool CanCloseDocument() => IsDocumentOpen;

        private void Undo()
        {
            _undoRedoService.UndoAsync();
        }

        private void Redo()
        {
            _undoRedoService.RedoAsync();
        }

        private void ZoomIn()
        {
            ZoomLevel = Math.Min(ZoomLevel + 25, 400);
        }

        private void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel - 25, 25);
        }

        private void SetZoomMode(ZoomMode mode)
        {
            // Implementation would calculate zoom based on page size and viewport
            StatusMessage = $"Zoom mode set to {mode}";
        }

        private void OnUndoRedoStateChanged(object sender, UndoRedoStateChangedEventArgs e)
        {
            (UndoCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (RedoCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        public class RecentDocument
        {
            public string FilePath { get; set; }
            public string Name { get; set; }
            public DateTime LastAccessed { get; set; }
        }
    }
}