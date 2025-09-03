// MyPdfEditor.WPF/ViewModels/FieldViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyPdfEditor.Core.Models.Enums;
using MyPdfEditor.Core.Models.Forms;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class FieldViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private FieldType _fieldType;

        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private int _pageNumber;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private bool _isReadOnly;

        [ObservableProperty]
        private bool _isRequired;

        [ObservableProperty]
        private string _validationPattern;

        [ObservableProperty]
        private string _borderColor;

        [ObservableProperty]
        private double _borderWidth;

        [ObservableProperty]
        private string _backgroundColor;

        [ObservableProperty]
        private string _toolTip;

        public FieldViewModel()
        {
            // Default constructor for design-time data
        }

        public FieldViewModel(FormField field)
        {
            UpdateFromField(field);
        }

        public virtual void UpdateFromField(FormField field)
        {
            Id = field.Id;
            Name = field.Name;
            FieldType = field.FieldType;
            X = field.X;
            Y = field.Y;
            Width = field.Width;
            Height = field.Height;
            PageNumber = field.PageNumber;
            IsVisible = field.IsVisible;
            IsReadOnly = field.IsReadOnly;
            IsRequired = field.IsRequired;
            ValidationPattern = field.ValidationPattern;
            BorderColor = field.BorderColor;
            BorderWidth = field.BorderWidth;
            BackgroundColor = field.BackgroundColor;
            ToolTip = field.ToolTip;
        }

        public virtual void ApplyToField(FormField field)
        {
            field.Name = Name;
            field.X = X;
            field.Y = Y;
            field.Width = Width;
            field.Height = Height;
            field.IsVisible = IsVisible;
            field.IsReadOnly = IsReadOnly;
            field.IsRequired = IsRequired;
            field.ValidationPattern = ValidationPattern;
            field.BorderColor = BorderColor;
            field.BorderWidth = BorderWidth;
            field.BackgroundColor = BackgroundColor;
            field.ToolTip = ToolTip;
        }

        public virtual FormField CreateFieldInstance()
        {
            return FieldType switch
            {
                FieldType.Text => new TextField(),
                FieldType.Checkbox => new CheckboxField(),
                FieldType.RadioButton => new RadioButtonField(),
                FieldType.ComboBox => new ComboBoxField(),
                FieldType.ListBox => new ListBoxField(),
                FieldType.Button => new ButtonField(),
                _ => new TextField() // Default fallback
            };
        }

        // Property change handlers
        partial void OnNameChanged(string value) => OnPropertyChanged(nameof(DisplayName));
        partial void OnFieldTypeChanged(FieldType value) => OnPropertyChanged(nameof(DisplayName));

        public string DisplayName => !string.IsNullOrEmpty(Name) ? Name : $"{FieldType}_{Id?.Substring(0, 8) ?? "new"}";

        public virtual bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;

            if (Width <= 0 || Height <= 0)
                return false;

            if (X < 0 || Y < 0)
                return false;

            return true;
        }

        public virtual object GetValue() => null;
        public virtual void SetValue(object value) { }
    }
}