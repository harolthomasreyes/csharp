// MyPdfEditor.WPF/Converters/PointsToPixelsConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using MyPdfEditor.Core.Utilities;

namespace MyPdfEditor.WPF.Converters
{
    public class PointsToPixelsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double points)
            {
                return CoordinateConverter.PointsToPixels(points);
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double pixels)
            {
                return CoordinateConverter.PixelsToPoints(pixels);
            }
            return 0.0;
        }
    }
}