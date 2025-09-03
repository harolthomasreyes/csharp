// MyPdfEditor.WPF/ViewModels/FormFieldViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyPdfEditor.Core.Models.Forms;
using System.Windows.Media.Media3D;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class FormFieldViewModel : ObservableObject
    {
        protected readonly FormField _field;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private bool _isReadOnly;

        public FormField Model => _field;

        public FormFieldViewModel(FormField field)
        {
            _field = field;
            UpdateFromModel();
        }

        protected virtual void UpdateFromModel()
        {
            Name = _field.Name;
            X = _field.X;
            Y = _field.Y;
            Width = _field.Width;
            Height = _field.Height;
            IsVisible = _field.IsVisible;
            IsReadOnly = _field.IsReadOnly;
        }

        protected virtual void UpdateModel()
        {
            _field.Name = Name;
            _field.X = X;
            _field.Y = Y;
            _field.Width = Width;
            _field.Height = Height;
            _field.IsVisible = IsVisible;
            _field.IsReadOnly = IsReadOnly;
        }

        partial void OnNameChanged(string value) => UpdateModel();
        partial void OnXChanged(double value) => UpdateModel();
        partial void OnYChanged(double value) => UpdateModel();
        partial void OnWidthChanged(double value) => UpdateModel();
        partial void OnHeightChanged(double value) => UpdateModel();
        partial void OnIsVisibleChanged(bool value) => UpdateModel();
        partial void OnIsReadOnlyChanged(bool value) => UpdateModel();
    }
}