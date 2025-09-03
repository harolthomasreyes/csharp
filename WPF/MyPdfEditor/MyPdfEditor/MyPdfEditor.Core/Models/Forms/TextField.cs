using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Forms
{
    public class TextField : FormField
    {
        private string _text = string.Empty;

        public TextField()
        {
            FieldType = FieldType.Text;
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnValueChanged();
            }
        }

        public string FontFamily { get; set; } = "Arial";
        public double FontSize { get; set; } = 12;
        public string FontColor { get; set; } = "#000000";
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
        public bool IsMultiline { get; set; }
        public int MaxLength { get; set; } = 0; // 0 = unlimited

        public override object GetValue() => Text;

        public override void SetValue(object value)
        {
            if (value is string stringValue)
            {
                Text = stringValue;
            }
        }

        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right,
        Justified
    }
}