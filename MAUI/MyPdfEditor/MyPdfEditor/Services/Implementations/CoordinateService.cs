using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPdfEditor.Services.Implementations
{
    public class CoordinateService : ICoordinateService
    {
        public Point ScreenToPdf(Point screenPoint, double zoomLevel, Point documentOffset)
        {
            return new Point(
                (screenPoint.X - documentOffset.X) / zoomLevel,
                (screenPoint.Y - documentOffset.Y) / zoomLevel
            );
        }

        public Point PdfToScreen(Point pdfPoint, double zoomLevel, Point documentOffset)
        {
            return new Point(
                pdfPoint.X * zoomLevel + documentOffset.X,
                pdfPoint.Y * zoomLevel + documentOffset.Y
            );
        }

        public Rect ScreenToPdf(Rect screenRect, double zoomLevel, Point documentOffset)
        {
            return new Rect(
                ScreenToPdf(screenRect.TopLeft, zoomLevel, documentOffset),
                ScreenToPdf(screenRect.BottomRight, zoomLevel, documentOffset)
            );
        }

        public Rect PdfToScreen(Rect pdfRect, double zoomLevel, Point documentOffset)
        {
            return new Rect(
                PdfToScreen(pdfRect.TopLeft, zoomLevel, documentOffset),
                PdfToScreen(pdfRect.BottomRight, zoomLevel, documentOffset)
            );
        }

        public bool IsPointInElement(Point point, PdfElement element, double zoomLevel, Point offset)
        {
            var screenRect = PdfToScreen(new Rect(element.X, element.Y, element.Width, element.Height), zoomLevel, offset);
            return screenRect.Contains(point);
        }
    }
}
