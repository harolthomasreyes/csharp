using System.Globalization;
using System.Windows.Data;

namespace MyPdfEditor.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                double trueOpacity = 1.0;
                double falseOpacity = 0.5;

                if (parameter is string param)
                {
                    var parts = param.Split(';');
                    if (parts.Length >= 2)
                    {
                        if (double.TryParse(parts[0], out double trueVal) && double.TryParse(parts[1], out double falseVal))
                        {
                            trueOpacity = trueVal;
                            falseOpacity = falseVal;
                        }
                    }
                }

                return boolValue ? trueOpacity : falseOpacity;
            }
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
