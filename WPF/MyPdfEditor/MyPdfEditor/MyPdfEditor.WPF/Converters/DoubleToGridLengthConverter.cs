// MyPdfEditor.WPF/Converters/DoubleToGridLengthConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.WPF.Converters
{
    public class DoubleToGridLengthConverter : IValueConverter
    {
        public GridUnitType UnitType { get; set; } = GridUnitType.Pixel;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return new GridLength(doubleValue, UnitType);
            }
            return new GridLength(1, UnitType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridLength gridLength)
            {
                return gridLength.Value;
            }
            return 0.0;
        }
    }
}