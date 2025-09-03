using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = true;

            if (value is bool boolValue)
            {
                isVisible = boolValue;
            }
            else if (value is string stringValue)
            {
                isVisible = !string.IsNullOrEmpty(stringValue);
            }
            else if (value is int intValue)
            {
                isVisible = intValue > 0;
            }
            else
            {
                isVisible = value != null;
            }

            if (parameter is string param && param.ToLower() == "inverse")
            {
                isVisible = !isVisible;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
