// MyPdfEditor.Core/Utilities/ExtensionMethods.cs
using System;
using System.Collections.Generic;
using System.Linq;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.Core.Utilities
{
    public static class ExtensionMethods
    {
        // String extensions
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || maxLength <= 0)
                return value;

            return value.Length <= maxLength ?
                value :
                value.Substring(0, maxLength) + suffix;
        }

        // Collection extensions
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null || action == null)
                return;

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> collection) where T : class
        {
            return collection?.Where(item => item != null) ?? Enumerable.Empty<T>();
        }

        // Document extensions
        public static PdfPage GetPageByNumber(this PdfDocument document, int pageNumber)
        {
            return document?.Pages?.FirstOrDefault(p => p.PageNumber == pageNumber);
        }

        public static FormField FindFieldById(this PdfDocument document, string fieldId)
        {
            if (document?.Pages == null)
                return null;

            foreach (var page in document.Pages)
            {
                var field = page.FormFields.FirstOrDefault(f => f.Id == fieldId);
                if (field != null)
                    return field;
            }

            return null;
        }

        public static IEnumerable<FormField> GetAllFields(this PdfDocument document)
        {
            if (document?.Pages == null)
                return Enumerable.Empty<FormField>();

            return document.Pages.SelectMany(page => page.FormFields);
        }

        public static int FieldCount(this PdfDocument document)
        {
            return document?.Pages?.Sum(page => page.FormFields.Count) ?? 0;
        }

        // Field extensions
        public static bool IsTextType(this FormField field)
        {
            return field is TextField;
        }

        public static bool IsCheckboxType(this FormField field)
        {
            return field is CheckboxField;
        }

        public static bool IsRadioButtonType(this FormField field)
        {
            return field is RadioButtonField;
        }

        public static bool IsComboBoxType(this FormField field)
        {
            return field is ComboBoxField;
        }

        public static bool IsListBoxType(this FormField field)
        {
            return field is ListBoxField;
        }

        public static bool IsButtonType(this FormField field)
        {
            return field is ButtonField;
        }

        public static string GetDisplayName(this FormField field)
        {
            return !string.IsNullOrEmpty(field.Name) ?
                field.Name :
                $"{field.FieldType}_{field.Id.Substring(0, 8)}";
        }

        // Math extensions
        public static double Clamp(this double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static double RoundTo(this double value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        // Color extensions (simplified)
        public static bool IsValidHexColor(this string color)
        {
            if (string.IsNullOrEmpty(color))
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(color,
                @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3}|[A-Fa-f0-9]{8})$");
        }

        // Enum extensions
        public static string ToFriendlyString(this Enum value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value.ToString(),
                @"([A-Z])",
                " $1").Trim();
        }

        // DateTime extensions
        public static string ToPdfDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public static DateTime FromPdfDateFormat(this string pdfDate)
        {
            if (DateTime.TryParseExact(pdfDate, "yyyyMMddHHmmss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var result))
            {
                return result;
            }
            return DateTime.MinValue;
        }
    }
}