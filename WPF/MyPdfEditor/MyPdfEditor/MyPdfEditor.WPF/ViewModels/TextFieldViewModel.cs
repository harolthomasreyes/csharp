// MyPdfEditor.WPF/ViewModels/TextFieldViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyPdfEditor.Core.Models.Forms;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class TextFieldViewModel : FormFieldViewModel
    {
        private readonly TextField _textField;

        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private string _fontFamily;

        [ObservableProperty]
        private double _fontSize;

        [ObservableProperty]
        private string _fontColor;

        [ObservableProperty]
        private bool _isMultiline;

        [ObservableProperty]
        private int _maxLength;

        public TextFieldViewModel(TextField field) : base(field)
        {
            _textField = field;
            UpdateFromModel();
        }

        protected override void UpdateFromModel()
        {
            base.UpdateFromModel();
            Text = _textField.Text;
            FontFamily = _textField.FontFamily;
            FontSize = _textField.FontSize;
            FontColor = _textField.FontColor;
            IsMultiline = _textField.IsMultiline;
            MaxLength = _textField.MaxLength;
        }

        protected override void UpdateModel()
        {
            base.UpdateModel();
            _textField.Text = Text;
            _textField.FontFamily = FontFamily;
            _textField.FontSize = FontSize;
            _textField.FontColor = FontColor;
            _textField.IsMultiline = IsMultiline;
            _textField.MaxLength = MaxLength;
        }

        partial void OnTextChanged(string value) => UpdateModel();
        partial void OnFontFamilyChanged(string value) => UpdateModel();
        partial void OnFontSizeChanged(double value) => UpdateModel();
        partial void OnFontColorChanged(string value) => UpdateModel();
        partial void OnIsMultilineChanged(bool value) => UpdateModel();
        partial void OnMaxLengthChanged(int value) => UpdateModel();
    }
}