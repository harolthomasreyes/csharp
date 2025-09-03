using MyPdfEditor.Core.Models.Enums;
using System.Globalization;
using System.Windows.Data;

namespace MyPdfEditor.Converters
{
    public class ElementTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PdfElementTypeEnum elementType)
            {
                return elementType switch
                {
                    PdfElementTypeEnum.TextField => "📝",
                    PdfElementTypeEnum.Checkbox => "☑️",
                    PdfElementTypeEnum.RadioButton => "🔘",
                    PdfElementTypeEnum.ComboBox => "▼",
                    PdfElementTypeEnum.ListBox => "📋",
                    PdfElementTypeEnum.Button => "🔘",
                    _ => "❓"
                };
            }
            return "❓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
