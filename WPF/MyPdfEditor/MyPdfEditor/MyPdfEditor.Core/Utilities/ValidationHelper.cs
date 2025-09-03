// MyPdfEditor.Core/Utilities/ValidationHelper.cs
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.Core.Utilities
{
    public static class ValidationHelper
    {
        // Common validation patterns
        public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string PhonePattern = @"^(\+\d{1,3})?[\s\-]?\(?\d{1,4}\)?[\s\-]?\d{1,4}[\s\-]?\d{1,9}$";
        public const string UrlPattern = @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";
        public const string NumericPattern = @"^-?\d*(\.\d+)?$";
        public const string IntegerPattern = @"^-?\d+$";

        // Document validation
        public static (bool IsValid, List<string> Errors) ValidateDocument(PdfDocument document)
        {
            var errors = new List<string>();

            if (document == null)
            {
                errors.Add("Document is null");
                return (false, errors);
            }

            if (string.IsNullOrWhiteSpace(document.DocumentName))
            {
                errors.Add("Document name is required");
            }

            if (document.Pages == null || document.Pages.Count == 0)
            {
                errors.Add("Document must contain at least one page");
            }
            else
            {
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    var pageErrors = ValidatePage(document.Pages[i], i + 1);
                    errors.AddRange(pageErrors);
                }
            }

            if (document.Security != null)
            {
                var securityErrors = ValidateSecurityOptions(document.Security);
                errors.AddRange(securityErrors);
            }

            return (errors.Count == 0, errors);
        }

        public static List<string> ValidatePage(PdfPage page, int pageNumber)
        {
            var errors = new List<string>();

            if (page == null)
            {
                errors.Add($"Page {pageNumber} is null");
                return errors;
            }

            if (page.Width <= 0)
            {
                errors.Add($"Page {pageNumber}: Width must be greater than 0");
            }

            if (page.Height <= 0)
            {
                errors.Add($"Page {pageNumber}: Height must be greater than 0");
            }

            if (page.FormFields != null)
            {
                foreach (var field in page.FormFields)
                {
                    var fieldErrors = ValidateField(field, pageNumber);
                    errors.AddRange(fieldErrors);
                }
            }

            return errors;
        }

        // Field validation
        public static List<string> ValidateField(FormField field, int pageNumber)
        {
            var errors = new List<string>();

            if (field == null)
            {
                errors.Add($"Page {pageNumber}: Field is null");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(field.Name))
            {
                errors.Add($"Page {pageNumber}: Field name is required");
            }

            if (field.X < 0)
            {
                errors.Add($"Page {pageNumber}: Field '{field.Name}' X position cannot be negative");
            }

            if (field.Y < 0)
            {
                errors.Add($"Page {pageNumber}: Field '{field.Name}' Y position cannot be negative");
            }

            if (field.Width <= 0)
            {
                errors.Add($"Page {pageNumber}: Field '{field.Name}' width must be greater than 0");
            }

            if (field.Height <= 0)
            {
                errors.Add($"Page {pageNumber}: Field '{field.Name}' height must be greater than 0");
            }

            // Field type specific validation
            switch (field)
            {
                case TextField textField:
                    errors.AddRange(ValidateTextField(textField, pageNumber));
                    break;
                case CheckboxField checkbox:
                    errors.AddRange(ValidateCheckboxField(checkbox, pageNumber));
                    break;
                case RadioButtonField radio:
                    errors.AddRange(ValidateRadioButtonField(radio, pageNumber));
                    break;
                case ComboBoxField combo:
                    errors.AddRange(ValidateComboBoxField(combo, pageNumber));
                    break;
                case ListBoxField list:
                    errors.AddRange(ValidateListBoxField(list, pageNumber));
                    break;
                case ButtonField button:
                    errors.AddRange(ValidateButtonField(button, pageNumber));
                    break;
            }

            return errors;
        }

        private static List<string> ValidateTextField(TextField field, int pageNumber)
        {
            var errors = new List<string>();

            if (field.FontSize <= 0)
            {
                errors.Add($"Page {pageNumber}: Text field '{field.Name}' font size must be greater than 0");
            }

            if (field.MaxLength < 0)
            {
                errors.Add($"Page {pageNumber}: Text field '{field.Name}' max length cannot be negative");
            }

            if (!string.IsNullOrEmpty(field.ValidationPattern))
            {
                try
                {
                    _ = new Regex(field.ValidationPattern);
                }
                catch
                {
                    errors.Add($"Page {pageNumber}: Text field '{field.Name}' has invalid validation pattern");
                }
            }

            return errors;
        }

        private static List<string> ValidateCheckboxField(CheckboxField field, int pageNumber)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(field.CheckedSymbol))
            {
                errors.Add($"Page {pageNumber}: Checkbox '{field.Name}' checked symbol is required");
            }

            return errors;
        }

        private static List<string> ValidateRadioButtonField(RadioButtonField field, int pageNumber)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(field.GroupName))
            {
                errors.Add($"Page {pageNumber}: Radio button '{field.Name}' group name is required");
            }

            if (string.IsNullOrEmpty(field.SelectedSymbol))
            {
                errors.Add($"Page {pageNumber}: Radio button '{field.Name}' selected symbol is required");
            }

            return errors;
        }

        private static List<string> ValidateComboBoxField(ComboBoxField field, int pageNumber)
        {
            var errors = new List<string>();

            if (field.Items == null)
            {
                errors.Add($"Page {pageNumber}: Combo box '{field.Name}' items collection cannot be null");
            }

            return errors;
        }

        private static List<string> ValidateListBoxField(ListBoxField field, int pageNumber)
        {
            var errors = new List<string>();

            if (field.Items == null)
            {
                errors.Add($"Page {pageNumber}: List box '{field.Name}' items collection cannot be null");
            }

            return errors;
        }

        private static List<string> ValidateButtonField(ButtonField field, int pageNumber)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(field.Caption))
            {
                errors.Add($"Page {pageNumber}: Button '{field.Name}' caption is required");
            }

            if (field.FontSize <= 0)
            {
                errors.Add($"Page {pageNumber}: Button '{field.Name}' font size must be greater than 0");
            }

            return errors;
        }

        // Security validation
        public static List<string> ValidateSecurityOptions(SecurityOptions security)
        {
            var errors = new List<string>();

            if (security.IsEncrypted)
            {
                if (string.IsNullOrEmpty(security.Password))
                {
                    errors.Add("Password is required for encrypted documents");
                }
                else if (!security.ValidatePasswordStrength())
                {
                    errors.Add("Password does not meet strength requirements");
                }

                if (security.KeyLength != 128 && security.KeyLength != 256)
                {
                    errors.Add("Key length must be 128 or 256 bits");
                }
            }

            return errors;
        }

        // Pattern validation
        public static bool ValidatePattern(string input, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            try
            {
                return Regex.IsMatch(input ?? string.Empty, pattern);
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidateEmail(string email)
        {
            return ValidatePattern(email, EmailPattern);
        }

        public static bool ValidatePhone(string phone)
        {
            return ValidatePattern(phone, PhonePattern);
        }

        public static bool ValidateUrl(string url)
        {
            return ValidatePattern(url, UrlPattern);
        }

        public static bool IsNumeric(string input)
        {
            return ValidatePattern(input, NumericPattern);
        }

        public static bool IsInteger(string input)
        {
            return ValidatePattern(input, IntegerPattern);
        }

        // Value validation
        public static bool ValidateRequiredField(object value, bool isRequired)
        {
            if (!isRequired) return true;

            return value switch
            {
                null => false,
                string str => !string.IsNullOrWhiteSpace(str),
                _ => true
            };
        }

        public static bool ValidateLength(string value, int maxLength)
        {
            if (maxLength <= 0) return true;
            return value?.Length <= maxLength;
        }

        public static bool ValidateRange(double value, double? min, double? max)
        {
            if (min.HasValue && value < min.Value) return false;
            if (max.HasValue && value > max.Value) return false;
            return true;
        }

        // File validation
        public static bool IsValidPdfExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            return extension == ".pdf";
        }

        public static bool IsValidImageExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            return extension == ".png" || extension == ".jpg" || extension == ".jpeg" ||
                   extension == ".bmp" || extension == ".tiff" || extension == ".tif";
        }
    }
}