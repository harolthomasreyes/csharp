// MyPdfEditor.WPF/Converters/ButtonActionConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.WPF.Converters
{
    public class ButtonActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ButtonAction action)
            {
                return action switch
                {
                    ButtonAction.None => "None",
                    ButtonAction.SubmitForm => "Submit Form",
                    ButtonAction.ResetForm => "Reset Form",
                    ButtonAction.PrintDocument => "Print Document",
                    ButtonAction.OpenUrl => "Open URL",
                    ButtonAction.ExecuteScript => "Execute Script",
                    ButtonAction.Custom => "Custom Action",
                    _ => "None"
                };
            }
            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string actionString)
            {
                return actionString.ToLower() switch
                {
                    "none" => ButtonAction.None,
                    "submit form" => ButtonAction.SubmitForm,
                    "reset form" => ButtonAction.ResetForm,
                    "print document" => ButtonAction.PrintDocument,
                    "open url" => ButtonAction.OpenUrl,
                    "execute script" => ButtonAction.ExecuteScript,
                    "custom action" => ButtonAction.Custom,
                    _ => ButtonAction.None
                };
            }
            return ButtonAction.None;
        }
    }
}