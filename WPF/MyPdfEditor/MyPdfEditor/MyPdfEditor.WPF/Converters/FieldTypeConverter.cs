// MyPdfEditor.WPF/Converters/FieldTypeConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.WPF.Converters
{
    public class FieldTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FieldType fieldType)
            {
                return fieldType switch
                {
                    FieldType.Text => "Text Field",
                    FieldType.Checkbox => "Checkbox",
                    FieldType.RadioButton => "Radio Button",
                    FieldType.ComboBox => "Combo Box",
                    FieldType.ListBox => "List Box",
                    FieldType.Button => "Button",
                    _ => "Unknown"
                };
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}