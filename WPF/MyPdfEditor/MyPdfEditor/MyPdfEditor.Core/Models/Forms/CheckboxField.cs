using MyPdfEditor.Core.Models.Enums;
namespace MyPdfEditor.Core.Models.Forms
{
    public class CheckboxField : FormField
    {
        private bool _isChecked;

        public CheckboxField()
        {
            FieldType = FieldType.Checkbox;
            Width = 15;
            Height = 15;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnValueChanged();
            }
        }

        public string CheckedSymbol { get; set; } = "✓";
        public string UncheckedSymbol { get; set; } = "";
        public string CheckMarkColor { get; set; } = "#000000";

        public override object GetValue() => IsChecked;

        public override void SetValue(object value)
        {
            if (value is bool boolValue)
            {
                IsChecked = boolValue;
            }
        }

        public void Toggle()
        {
            IsChecked = !IsChecked;
        }

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}