// MyPdfEditor.Core/Services/Implementations/PdfElementService.cs
using MyPdfEditor.Core.Contracts.Services;
using MyPdfEditor.Core.Models;
using MyPdfEditor.Core.Models.Enums;
using PdfSharp.Pdf;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class PdfElementService : IPdfElementService
    {
        public async Task<PdfElement> CreateElementAsync(PdfElementType type, double x, double y, double width, double height)
        {
            return await Task.Run(() => new PdfElement
            {
                Id = GenerateElementId(),
                Type = type,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });
        }

        public async Task<PdfElement> AddElementToDocumentAsync(PdfDocument document, PdfElement element)
        {
            return await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));
                if (element == null) throw new ArgumentNullException(nameof(element));

                element.Id = GenerateElementId();
                element.CreatedDate = DateTime.Now;
                element.ModifiedDate = DateTime.Now;

                document.FormElements.Add(element);
                document.ModifiedDate = DateTime.Now;

                return element;
            });
        }

        public async Task UpdateElementAsync(PdfDocument document, PdfElement element)
        {
            await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));
                if (element == null) throw new ArgumentNullException(nameof(element));

                var existingElement = document.FormElements.FirstOrDefault(e => e.Id == element.Id);
                if (existingElement == null)
                    throw new KeyNotFoundException($"Element with ID {element.Id} not found");

                existingElement.X = element.X;
                existingElement.Y = element.Y;
                existingElement.Width = element.Width;
                existingElement.Height = element.Height;
                existingElement.Text = element.Text;
                existingElement.IsChecked = element.IsChecked;
                existingElement.ModifiedDate = DateTime.Now;

                document.ModifiedDate = DateTime.Now;
            });
        }

        public async Task RemoveElementAsync(PdfDocument document, string elementId)
        {
            await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));
                if (string.IsNullOrEmpty(elementId)) throw new ArgumentNullException(nameof(elementId));

                var element = document.FormElements.FirstOrDefault(e => e.Id == elementId);
                if (element == null)
                    throw new KeyNotFoundException($"Element with ID {elementId} not found");

                document.FormElements.Remove(element);
                document.ModifiedDate = DateTime.Now;
            });
        }

        public async Task<PdfElement> GetElementAsync(PdfDocument document, string elementId)
        {
            return await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));

                return document.FormElements.FirstOrDefault(e => e.Id == elementId)
                    ?? throw new KeyNotFoundException($"Element with ID {elementId} not found");
            });
        }

        public async Task<IEnumerable<PdfElement>> GetAllElementsAsync(PdfDocument document)
        {
            return await Task.Run(() =>
            {
                if (document == null) throw new ArgumentNullException(nameof(document));
                return document.FormElements.ToList();
            });
        }

        public async Task<bool> ValidateElementPositionAsync(PdfDocument document, PdfElement element)
        {
            return await Task.Run(() =>
            {
                if (document == null || element == null) return false;

                // Check if element is within page bounds
                var page = document.Pages.FirstOrDefault(p => p.PageNumber == element.PageNumber);
                if (page == null) return false;

                return element.X >= 0 && element.Y >= 0 &&
                       element.X + element.Width <= page.Width &&
                       element.Y + element.Height <= page.Height;
            });
        }

        public async Task<bool> ElementExistsAsync(PdfDocument document, string elementId)
        {
            return await Task.Run(() =>
            {
                if (document == null) return false;
                return document.FormElements.Any(e => e.Id == elementId);
            });
        }

        private string GenerateElementId() => $"elem_{Guid.NewGuid():N}";
    }
}