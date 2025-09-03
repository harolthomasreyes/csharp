// MyPdfEditor.Core/Services/Implementations/FormFieldService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Forms;
using MyPdfEditor.Core.Services.Interfaces;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class FormFieldService : IFormFieldService
    {
        public async Task<FormField> CreateTextFieldAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height)
        {
            return await Task.Run(() =>
            {
                var field = new TextField
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    PageNumber = pageNumber,
                    Name = $"TextField_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };

                return field;
            });
        }

        public async Task<FormField> CreateCheckboxAsync(PdfDocument document, int pageNumber, double x, double y)
        {
            return await Task.Run(() =>
            {
                var field = new CheckboxField
                {
                    X = x,
                    Y = y,
                    PageNumber = pageNumber,
                    Name = $"Checkbox_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };

                return field;
            });
        }

        public async Task<FormField> CreateRadioButtonAsync(PdfDocument document, int pageNumber, double x, double y, string groupName)
        {
            return await Task.Run(() =>
            {
                var field = new RadioButtonField
                {
                    X = x,
                    Y = y,
                    PageNumber = pageNumber,
                    GroupName = groupName,
                    Name = $"RadioButton_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };

                return field;
            });
        }

        public async Task<FormField> CreateComboBoxAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height)
        {
            return await Task.Run(() =>
            {
                var field = new ComboBoxField
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    PageNumber = pageNumber,
                    Name = $"ComboBox_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };

                return field;
            });
        }

        public async Task<FormField> CreateListBoxAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height)
        {
            return await Task.Run(() =>
            {
                var field = new ListBoxField
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    PageNumber = pageNumber,
                    Name = $"ListBox_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };

                return field;
            });
        }

        public async Task<FormField> CreateButtonAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height)
        {
            return await Task.Run(() =>
            {
                var field = new ButtonField
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    PageNumber = pageNumber,
                    Name = $"Button_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                    Caption = "Button"
                };

                return field;
            });
        }

        public async Task<bool> AddFieldToPageAsync(PdfDocument document, int pageNumber, FormField field)
        {
            return await Task.Run(() =>
            {
                if (pageNumber < 1 || pageNumber > document.Pages.Count)
                    return false;

                var page = document.Pages[pageNumber - 1];
                page.AddField(field);
                document.IsModified = true;
                return true;
            });
        }

        public async Task<bool> RemoveFieldFromPageAsync(PdfDocument document, int pageNumber, string fieldId)
        {
            return await Task.Run(() =>
            {
                if (pageNumber < 1 || pageNumber > document.Pages.Count)
                    return false;

                var page = document.Pages[pageNumber - 1];
                var field = page.FormFields.FirstOrDefault(f => f.Id == fieldId);

                if (field != null)
                {
                    page.RemoveField(field);
                    document.IsModified = true;
                    return true;
                }

                return false;
            });
        }

        public async Task<FormField> GetFieldByIdAsync(PdfDocument document, string fieldId)
        {
            return await Task.Run(() =>
            {
                foreach (var page in document.Pages)
                {
                    var field = page.FormFields.FirstOrDefault(f => f.Id == fieldId);
                    if (field != null)
                        return field;
                }
                return null;
            });
        }

        public async Task<IEnumerable<FormField>> GetFieldsByPageAsync(PdfDocument document, int pageNumber)
        {
            return await Task.Run(() =>
            {
                if (pageNumber < 1 || pageNumber > document.Pages.Count)
                    return Enumerable.Empty<FormField>();

                return document.Pages[pageNumber - 1].FormFields.AsEnumerable();
            });
        }

        public async Task<bool> UpdateFieldPositionAsync(PdfDocument document, string fieldId, double newX, double newY)
        {
            return await Task.Run(() =>
            {
                var field = GetFieldByIdAsync(document, fieldId).Result;
                if (field == null)
                    return false;

                field.X = newX;
                field.Y = newY;
                document.IsModified = true;
                return true;
            });
        }

        public async Task<bool> UpdateFieldSizeAsync(PdfDocument document, string fieldId, double newWidth, double newHeight)
        {
            return await Task.Run(() =>
            {
                var field = GetFieldByIdAsync(document, fieldId).Result;
                if (field == null)
                    return false;

                field.Width = newWidth;
                field.Height = newHeight;
                document.IsModified = true;
                return true;
            });
        }

        public async Task<bool> ValidateFieldPositionAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height)
        {
            return await Task.Run(() =>
            {
                if (pageNumber < 1 || pageNumber > document.Pages.Count)
                    return false;

                var page = document.Pages[pageNumber - 1];
                return x >= 0 && y >= 0 &&
                       x + width <= page.Width &&
                       y + height <= page.Height;
            });
        }

        public async Task<bool> CheckFieldOverlapAsync(PdfDocument document, int pageNumber, FormField field)
        {
            return await Task.Run(() =>
            {
                if (pageNumber < 1 || pageNumber > document.Pages.Count)
                    return false;

                var page = document.Pages[pageNumber - 1];
                foreach (var existingField in page.FormFields)
                {
                    if (existingField.Id != field.Id && DoFieldsOverlap(existingField, field))
                        return true;
                }

                return false;
            });
        }

        public async Task<bool> IsFieldWithinPageBoundsAsync(PdfDocument document, int pageNumber, FormField field)
        {
            return await Task.Run(() =>
            {
                if (pageNumber < 1 || pageNumber > document.Pages.Count)
                    return false;

                var page = document.Pages[pageNumber - 1];
                return field.X >= 0 && field.Y >= 0 &&
                       field.X + field.Width <= page.Width &&
                       field.Y + field.Height <= page.Height;
            });
        }

        public async Task<bool> MoveFieldsToPageAsync(PdfDocument document, IEnumerable<string> fieldIds, int targetPageNumber)
        {
            return await Task.Run(() =>
            {
                if (targetPageNumber < 1 || targetPageNumber > document.Pages.Count)
                    return false;

                var targetPage = document.Pages[targetPageNumber - 1];
                var moved = false;

                foreach (var fieldId in fieldIds)
                {
                    var field = GetFieldByIdAsync(document, fieldId).Result;
                    if (field != null)
                    {
                        // Remove from current page
                        var currentPage = document.Pages[field.PageNumber - 1];
                        currentPage.RemoveField(field);

                        // Add to target page
                        field.PageNumber = targetPageNumber;
                        targetPage.AddField(field);
                        moved = true;
                    }
                }

                if (moved) document.IsModified = true;
                return moved;
            });
        }

        public async Task<bool> DeleteFieldsAsync(PdfDocument document, IEnumerable<string> fieldIds)
        {
            return await Task.Run(() =>
            {
                var deleted = false;

                foreach (var fieldId in fieldIds)
                {
                    var field = GetFieldByIdAsync(document, fieldId).Result;
                    if (field != null)
                    {
                        var page = document.Pages[field.PageNumber - 1];
                        page.RemoveField(field);
                        deleted = true;
                    }
                }

                if (deleted) document.IsModified = true;
                return deleted;
            });
        }

        public async Task<bool> DuplicateFieldsAsync(PdfDocument document, IEnumerable<string> fieldIds, int targetPageNumber)
        {
            return await Task.Run(() =>
            {
                if (targetPageNumber < 1 || targetPageNumber > document.Pages.Count)
                    return false;

                var targetPage = document.Pages[targetPageNumber - 1];
                var duplicated = false;

                foreach (var fieldId in fieldIds)
                {
                    var original = GetFieldByIdAsync(document, fieldId).Result;
                    if (original != null)
                    {
                        // Create a deep copy (simplified)
                        var copy = CreateFieldCopy(original);
                        copy.X += 10; // Offset slightly
                        copy.Y += 10;
                        copy.PageNumber = targetPageNumber;

                        targetPage.AddField(copy);
                        duplicated = true;
                    }
                }

                if (duplicated) document.IsModified = true;
                return duplicated;
            });
        }

        public async Task<bool> SetFieldValueAsync(PdfDocument document, string fieldId, object value)
        {
            return await Task.Run(() =>
            {
                var field = GetFieldByIdAsync(document, fieldId).Result;
                if (field == null)
                    return false;

                field.SetValue(value);
                document.IsModified = true;
                return true;
            });
        }

        public async Task<object> GetFieldValueAsync(PdfDocument document, string fieldId)
        {
            return await Task.Run(() =>
            {
                var field = GetFieldByIdAsync(document, fieldId).Result;
                return field?.GetValue();
            });
        }

        public async Task<bool> ValidateFieldValueAsync(PdfDocument document, string fieldId)
        {
            return await Task.Run(() =>
            {
                var field = GetFieldByIdAsync(document, fieldId).Result;
                return field?.Validate() ?? false;
            });
        }

        public async Task<bool> ResetFieldValueAsync(PdfDocument document, string fieldId)
        {
            return await Task.Run(() =>
            {
                var field = GetFieldByIdAsync(document, fieldId).Result;
                if (field == null)
                    return false;

                // Reset to default based on field type
                switch (field)
                {
                    case TextField textField:
                        textField.Text = string.Empty;
                        break;
                    case CheckboxField checkbox:
                        checkbox.IsChecked = false;
                        break;
                    case RadioButtonField radio:
                        radio.IsSelected = false;
                        break;
                    case ComboBoxField combo:
                        combo.SelectedItem = null;
                        break;
                    case ListBoxField list:
                        list.SelectedItems.Clear();
                        break;
                }

                document.IsModified = true;
                return true;
            });
        }

        private bool DoFieldsOverlap(FormField field1, FormField field2)
        {
            return field1.X < field2.X + field2.Width &&
                   field1.X + field1.Width > field2.X &&
                   field1.Y < field2.Y + field2.Height &&
                   field1.Y + field1.Height > field2.Y;
        }

        private FormField CreateFieldCopy(FormField original)
        {
            return original switch
            {
                TextField text => new TextField
                {
                    Name = $"{text.Name}_Copy",
                    X = text.X,
                    Y = text.Y,
                    Width = text.Width,
                    Height = text.Height,
                    Text = text.Text,
                    FontFamily = text.FontFamily,
                    FontSize = text.FontSize
                },
                CheckboxField checkbox => new CheckboxField
                {
                    Name = $"{checkbox.Name}_Copy",
                    X = checkbox.X,
                    Y = checkbox.Y,
                    IsChecked = checkbox.IsChecked
                },
                _ => throw new NotSupportedException($"Field type {original.GetType().Name} not supported for copying")
            };
        }
    }
}