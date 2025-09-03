// MyPdfEditor.Core/Models/Forms/FormFieldFactory.cs
using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Forms
{
    public static class FormFieldFactory
    {
        public static FormField CreateField(FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.Text => new TextField(),
                FieldType.Checkbox => new CheckboxField(),
                FieldType.RadioButton => new RadioButtonField(),
                FieldType.ComboBox => new ComboBoxField(),
                FieldType.ListBox => new ListBoxField(),
                FieldType.Button => new ButtonField(),
                _ => throw new System.ArgumentException($"Unknown field type: {fieldType}")
            };
        }

        public static FormField CreateField(FieldType fieldType, double x, double y, int pageNumber)
        {
            var field = CreateField(fieldType);
            field.X = x;
            field.Y = y;
            field.PageNumber = pageNumber;
            return field;
        }
    }
}