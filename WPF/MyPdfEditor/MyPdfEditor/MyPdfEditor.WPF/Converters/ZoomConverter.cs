// MyPdfEditor.WPF/Converters/ZoomConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using MyPdfEditor.Core.Utilities;

namespace MyPdfEditor.WPF.Converters
{
    public class ZoomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double zoomLevel)
            {
                return zoomLevel / 100.0; // Convert percentage to factor
            }
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double zoomFactor)
            {
                return zoomFactor * 100.0; // Convert factor to percentage
            }
            return 100.0;
        }
    }
}