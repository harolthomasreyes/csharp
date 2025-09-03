using System.Windows;

namespace MyPdfEditor.Services.Interfaces
{
    public interface ICoordinateService
    {
        Point ScreenToPdf(Point screenPoint, double zoomLevel, Point documentOffset);
        Point PdfToScreen(Point pdfPoint, double zoomLevel, Point documentOffset);
        Rect ScreenToPdf(Rect screenRect, double zoomLevel, Point documentOffset);
        Rect PdfToScreen(Rect pdfRect, double zoomLevel, Point documentOffset);
        bool IsPointInElement(Point point, PdfElement element, double zoomLevel, Point offset);
    }
}
