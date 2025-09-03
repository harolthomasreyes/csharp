using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPdfEditor.Converters
{ 
    public class ThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double thicknessValue)
            {
                return new Thickness(thicknessValue);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return thickness.Left;
            }
            return 0.0;
        }
    }
}
