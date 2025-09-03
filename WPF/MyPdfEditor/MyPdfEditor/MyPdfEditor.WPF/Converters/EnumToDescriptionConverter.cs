// MyPdfEditor.WPF/Converters/EnumToDescriptionConverter.cs
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace MyPdfEditor.WPF.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}