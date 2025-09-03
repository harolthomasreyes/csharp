// MyPdfEditor.WPF/Converters/TextAlignmentConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.WPF.Converters
{
    public class TextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string alignmentString)
            {
                return alignmentString.ToLower() switch
                {
                    "left" => TextAlignment.Left,
                    "center" => TextAlignment.Center,
                    "right" => TextAlignment.Right,
                    "justify" => TextAlignment.Justify,
                    _ => TextAlignment.Left
                };
            }
            return TextAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextAlignment alignment)
            {
                return alignment switch
                {
                    TextAlignment.Left => "Left",
                    TextAlignment.Center => "Center",
                    TextAlignment.Right => "Right",
                    TextAlignment.Justify => "Justify",
                    _ => "Left"
                };
            }
            return "Left";
        }
    }
}