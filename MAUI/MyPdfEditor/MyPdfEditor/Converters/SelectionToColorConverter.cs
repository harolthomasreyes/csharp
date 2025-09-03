using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyPdfEditor.Converters
{
    public class SelectionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Gray);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
