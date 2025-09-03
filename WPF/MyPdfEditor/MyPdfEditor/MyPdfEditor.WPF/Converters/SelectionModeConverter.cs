// MyPdfEditor.WPF/Converters/SelectionModeConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.WPF.Converters
{
    public class SelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelectionMode selectionMode)
            {
                return selectionMode switch
                {
                    SelectionMode.Single => "Single",
                    SelectionMode.Multiple => "Multiple",
                    _ => "Single"
                };
            }
            return "Single";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string modeString)
            {
                return modeString.ToLower() switch
                {
                    "single" => SelectionMode.Single,
                    "multiple" => SelectionMode.Multiple,
                    _ => SelectionMode.Single
                };
            }
            return SelectionMode.Single;
        }
    }
}