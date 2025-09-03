using System.Windows;
using System.Collections.Generic;

namespace MyPdfEditor.Services.Interfaces
{
    public interface IInputService
    {
        void HandleMouseDown(Point screenPoint, double zoomLevel, Point documentOffset, IEnumerable<PdfElement> elements);
        void HandleMouseMove(Point currentPoint, double zoomLevel, Point documentOffset);
        void HandleMouseUp();
    }
}
