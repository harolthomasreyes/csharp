using MyPdfEditor.Core.Models.Enums;
namespace MyPdfEditor.Core.Models.Forms
{
    public class RadioButtonField : FormField
    {
        private bool _isSelected;

        public RadioButtonField()
        {
            FieldType = FieldType.RadioButton;
            Width = 15;
            Height = 15;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnValueChanged();
            }
        }

        public string GroupName { get; set; } = string.Empty;
        public string SelectedSymbol { get; set; } = "●";
        public string UnselectedSymbol { get; set; } = "○";
        public string SelectionColor { get; set; } = "#000000";

        public override object GetValue() => IsSelected;

        public override void SetValue(object value)
        {
            if (value is bool boolValue)
            {
                IsSelected = boolValue;
            }
        }

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}