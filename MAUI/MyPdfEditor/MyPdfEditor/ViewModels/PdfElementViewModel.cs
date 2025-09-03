using MyPdfEditor.Core.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyPdfEditor.ViewModels
{
    public class PdfElementViewModel : INotifyPropertyChanged
    {
        private string _id;
        private PdfElementTypeEnum _type;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private string _text;
        private bool _isChecked;
        private bool _isSelected;
        private bool _isVisible = true;

        public string Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public PdfElementTypeEnum Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }

        public double X
        {
            get => _x;
            set => SetField(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetField(ref _y, value);
        }

        public double Width
        {
            get => _width;
            set => SetField(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetField(ref _height, value);
        }

        public string Text
        {
            get => _text;
            set => SetField(ref _text, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetField(ref _isChecked, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetField(ref _isVisible, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
