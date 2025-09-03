// MyPdfEditor.WPF/Converters/ColorToBrushConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyPdfEditor.WPF.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                try
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFromString(colorString);
                }
                catch
                {
                    return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }
            return "#000000";
        }
    }
}