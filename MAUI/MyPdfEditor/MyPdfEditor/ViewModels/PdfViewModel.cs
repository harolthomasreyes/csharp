using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyPdfEditor.ViewModels
{
    public class PdfViewModel : INotifyPropertyChanged
    {
        private PdfDocument _currentDocument;
        private double _zoomLevel = 1.0;
        private string _selectedElementId;

        public PdfDocument CurrentDocument
        {
            get => _currentDocument;
            set => SetField(ref _currentDocument, value);
        }

        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                if (SetField(ref _zoomLevel, value))
                {
                    ZoomChanged?.Invoke(this, value);
                }
            }
        }

        public string SelectedElementId
        {
            get => _selectedElementId;
            set
            {
                if (SetField(ref _selectedElementId, value))
                {
                    SelectionUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ObservableCollection<PdfElementViewModel> Elements { get; } = new ObservableCollection<PdfElementViewModel>();

        public event EventHandler<double> ZoomChanged;
        public event EventHandler ElementsChanged;
        public event EventHandler SelectionUpdated;

        public void LoadDocument(PdfDocument document)
        {
            CurrentDocument = document;
            Elements.Clear();

            foreach (var element in document.FormElements)
            {
                Elements.Add(new PdfElementViewModel
                {
                    Id = element.Id,
                    Type = MapElementType(element.Type),
                    X = element.X,
                    Y = element.Y,
                    Width = element.Width,
                    Height = element.Height,
                    Text = element.Text,
                    IsChecked = element.IsChecked
                });
            }

            ElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddElement(PdfElementViewModel element)
        {
            Elements.Add(element);
            ElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveElement(string elementId)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].Id == elementId)
                {
                    Elements.RemoveAt(i);
                    ElementsChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        public void UpdateElementPosition(string elementId, double x, double y)
        {
            foreach (var element in Elements)
            {
                if (element.Id == elementId)
                {
                    element.X = x;
                    element.Y = y;
                    ElementsChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        private PdfElementType MapElementType(Core.Models.PdfElementType coreType)
        {
            return coreType switch
            {
                Core.Models.PdfElementType.TextField => PdfElementType.TextField,
                Core.Models.PdfElementType.Checkbox => PdfElementType.Checkbox,
                Core.Models.PdfElementType.RadioButton => PdfElementType.RadioButton,
                Core.Models.PdfElementType.ComboBox => PdfElementType.ComboBox,
                Core.Models.PdfElementType.ListBox => PdfElementType.ListBox,
                Core.Models.PdfElementType.Button => PdfElementType.Button,
                _ => PdfElementType.TextField
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
