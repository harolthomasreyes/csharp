using System.Globalization;
using System.Windows.Data;

namespace MyPdfEditor.Converters
{
    public class CoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double coordinate && parameter is string operation)
            {
                return operation.ToLower() switch
                {
                    "round" => Math.Round(coordinate),
                    "floor" => Math.Floor(coordinate),
                    "ceiling" => Math.Ceiling(coordinate),
                    _ => coordinate
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
