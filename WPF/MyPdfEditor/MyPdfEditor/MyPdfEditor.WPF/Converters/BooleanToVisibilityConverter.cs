// MyPdfEditor.WPF/Converters/BooleanToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.WPF.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
                flag = (bool)value;
            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible;
            return false;
        }
    }
}