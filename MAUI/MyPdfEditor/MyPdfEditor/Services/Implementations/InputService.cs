using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPdfEditor.Services.Implementations
{
    public class InputService : IInputService
    {
        private readonly ICoordinateService _coordinateService;
        private readonly ISelectionService _selectionService;

        private Point _dragStartPoint;
        private bool _isDragging;

        public InputService(ICoordinateService coordinateService, ISelectionService selectionService)
        {
            _coordinateService = coordinateService;
            _selectionService = selectionService;
        }

        public void HandleMouseDown(Point screenPoint, double zoomLevel, Point documentOffset, IEnumerable<PdfElement> elements)
        {
            _dragStartPoint = screenPoint;

            var pdfPoint = _coordinateService.ScreenToPdf(screenPoint, zoomLevel, documentOffset);
            var elementUnderCursor = FindElementAtPoint(pdfPoint, elements);

            if (elementUnderCursor != null)
            {
                _selectionService.SelectElement(elementUnderCursor.Id);
                _isDragging = true;
            }
            else
            {
                _selectionService.ClearSelection();
            }
        }

        public void HandleMouseMove(Point currentPoint, double zoomLevel, Point documentOffset)
        {
            if (_isDragging)
            {
                var offset = currentPoint - _dragStartPoint;
                // Handle drag operation
            }
        }

        public void HandleMouseUp()
        {
            _isDragging = false;
        }

        private PdfElement FindElementAtPoint(Point point, IEnumerable<PdfElement> elements)
        {
            return elements.FirstOrDefault(element =>
                point.X >= element.X && point.X <= element.X + element.Width &&
                point.Y >= element.Y && point.Y <= element.Y + element.Height);
        }
    }
}
