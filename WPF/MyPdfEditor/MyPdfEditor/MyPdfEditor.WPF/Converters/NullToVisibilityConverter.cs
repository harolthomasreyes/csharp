// MyPdfEditor.WPF/Converters/NullToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.WPF.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public bool UseHidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNull = value == null;
            if (Invert) isNull = !isNull;

            return isNull ? Visibility.Visible :
                (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}