using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Forms
{
    public abstract class FormField
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string ToolTip { get; set; } = string.Empty;
        public FieldType FieldType { get; protected set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; } = 100;
        public double Height { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
        public bool IsVisible { get; set; } = true;
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
        public string ValidationPattern { get; set; } = string.Empty;
        public string BorderColor { get; set; } = "#000000";
        public double BorderWidth { get; set; } = 1;
        public string BackgroundColor { get; set; } = "#FFFFFF";

        public abstract object GetValue();
        public abstract void SetValue(object value);

        public virtual bool Validate()
        {
            if (IsRequired && GetValue() == null)
                return false;

            if (!string.IsNullOrEmpty(ValidationPattern))
            {
                // Regex validation (implement as needed)
            }

            return true;
        }

        public bool ContainsPoint(double pointX, double pointY)
        {
            return pointX >= X && pointX <= X + Width &&
                   pointY >= Y && pointY <= Y + Height;
        }
    }
}