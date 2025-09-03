// MyPdfEditor.Core/Services/Interfaces/IFormFieldService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Forms;

namespace MyPdfEditor.Core.Services.Interfaces
{
    public interface IFormFieldService
    {
        // Field creation
        Task<FormField> CreateTextFieldAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height);
        Task<FormField> CreateCheckboxAsync(PdfDocument document, int pageNumber, double x, double y);
        Task<FormField> CreateRadioButtonAsync(PdfDocument document, int pageNumber, double x, double y, string groupName);
        Task<FormField> CreateComboBoxAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height);
        Task<FormField> CreateListBoxAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height);
        Task<FormField> CreateButtonAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height);

        // Field manipulation
        Task<bool> AddFieldToPageAsync(PdfDocument document, int pageNumber, FormField field);
        Task<bool> RemoveFieldFromPageAsync(PdfDocument document, int pageNumber, string fieldId);
        Task<FormField> GetFieldByIdAsync(PdfDocument document, string fieldId);
        Task<IEnumerable<FormField>> GetFieldsByPageAsync(PdfDocument document, int pageNumber);
        Task<bool> UpdateFieldPositionAsync(PdfDocument document, string fieldId, double newX, double newY);
        Task<bool> UpdateFieldSizeAsync(PdfDocument document, string fieldId, double newWidth, double newHeight);

        // Field validation
        Task<bool> ValidateFieldPositionAsync(PdfDocument document, int pageNumber, double x, double y, double width, double height);
        Task<bool> CheckFieldOverlapAsync(PdfDocument document, int pageNumber, FormField field);
        Task<bool> IsFieldWithinPageBoundsAsync(PdfDocument document, int pageNumber, FormField field);

        // Batch operations
        Task<bool> MoveFieldsToPageAsync(PdfDocument document, IEnumerable<string> fieldIds, int targetPageNumber);
        Task<bool> DeleteFieldsAsync(PdfDocument document, IEnumerable<string> fieldIds);
        Task<bool> DuplicateFieldsAsync(PdfDocument document, IEnumerable<string> fieldIds, int targetPageNumber);

        // Field properties
        Task<bool> SetFieldValueAsync(PdfDocument document, string fieldId, object value);
        Task<object> GetFieldValueAsync(PdfDocument document, string fieldId);
        Task<bool> ValidateFieldValueAsync(PdfDocument document, string fieldId);
        Task<bool> ResetFieldValueAsync(PdfDocument document, string fieldId);
    }
}