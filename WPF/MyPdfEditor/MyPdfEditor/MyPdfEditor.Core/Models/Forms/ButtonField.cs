using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Forms
{
    public class ButtonField : FormField
    {
        public ButtonField()
        {
            FieldType = FieldType.Button;
            Height = 30;
        }

        public string Caption { get; set; } = "Button";
        public string FontFamily { get; set; } = "Arial";
        public double FontSize { get; set; } = 12;
        public string FontColor { get; set; } = "#000000";
        public string HoverColor { get; set; } = "#E0E0E0";
        public string PressedColor { get; set; } = "#C0C0C0";
        public TextAlignment Alignment { get; set; } = TextAlignment.Center;

        public ButtonAction Action { get; set; } = ButtonAction.None;
        public string ActionParameter { get; set; } = string.Empty;

        public event EventHandler Click;

        public void PerformClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        public override object GetValue() => null; // Buttons don't hold values

        public override void SetValue(object value)
        {
            // Buttons don't hold values
        }
    }

    public enum ButtonAction
    {
        None,
        SubmitForm,
        ResetForm,
        PrintDocument,
        OpenUrl,
        ExecuteScript,
        Custom
    }
}