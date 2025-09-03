// MyPdfEditor.WPF/Converters/MultiBooleanToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.WPF.Converters
{
    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public bool RequireAllTrue { get; set; } = true;
        public bool Invert { get; set; }
        public bool UseHidden { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Any(v => v == null || v == DependencyProperty.UnsetValue))
                return Visibility.Collapsed;

            var results = values.Select(v => v is bool b && b).ToArray();

            bool result;
            if (RequireAllTrue)
            {
                result = results.All(r => r);
            }
            else
            {
                result = results.Any(r => r);
            }

            if (Invert) result = !result;

            return result ? Visibility.Visible :
                (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}