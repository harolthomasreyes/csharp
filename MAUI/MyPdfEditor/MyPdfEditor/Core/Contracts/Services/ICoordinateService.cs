using System.Drawing;

namespace MyPdfEditor.Core.Contracts.Services
{
    public interface ICoordinateService
    {
        PointF ScreenToPdf(Point screenPoint, float zoomLevel, PointF documentOffset);
        Point PdfToScreen(PointF pdfPoint, float zoomLevel, PointF documentOffset);
        RectangleF ScreenToPdf(Rectangle screenRect, float zoomLevel, PointF documentOffset);
        Rectangle PdfToScreen(RectangleF pdfRect, float zoomLevel, PointF documentOffset);
        bool IsPointInElement(PointF point, PdfElement element);
        PointF CalculateCenterPoint(PdfElement element);
        PointF CalculateRelativePosition(PointF absolutePoint, PdfElement parentElement);
    }
}
