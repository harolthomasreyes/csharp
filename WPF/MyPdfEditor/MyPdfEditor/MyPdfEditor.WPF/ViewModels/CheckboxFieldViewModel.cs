// MyPdfEditor.WPF/ViewModels/CheckboxFieldViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class CheckboxFieldViewModel : FormFieldViewModel
    {
        private readonly CheckboxField _checkbox;

        [ObservableProperty]
        private bool _isChecked;

        [ObservableProperty]
        private string _checkedSymbol;

        [ObservableProperty]
        private string _checkMarkColor;

        public CheckboxFieldViewModel(CheckboxField field) : base(field)
        {
            _checkbox = field;
            UpdateFromModel();
        }

        protected override void UpdateFromModel()
        {
            base.UpdateFromModel();
            IsChecked = _checkbox.IsChecked;
            CheckedSymbol = _checkbox.CheckedSymbol;
            CheckMarkColor = _checkbox.CheckMarkColor;
        }

        protected override void UpdateModel()
        {
            base.UpdateModel();
            _checkbox.IsChecked = IsChecked;
            _checkbox.CheckedSymbol = CheckedSymbol;
            _checkbox.CheckMarkColor = CheckMarkColor;
        }

        partial void OnIsCheckedChanged(bool value) => UpdateModel();
        partial void OnCheckedSymbolChanged(string value) => UpdateModel();
        partial void OnCheckMarkColorChanged(string value) => UpdateModel();
    }
}