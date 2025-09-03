// MyPdfEditor.Core/Services/Implementations/CoordinateService.cs
using MyPdfEditor.Core.Contracts.Services;
using System.Drawing;

namespace MyPdfEditor.Core.Services.Implementations
{
    public class CoordinateService : ICoordinateService
    {
        public PointF ScreenToPdf(Point screenPoint, float zoomLevel, PointF documentOffset)
        {
            return new PointF(
                (screenPoint.X - documentOffset.X) / zoomLevel,
                (screenPoint.Y - documentOffset.Y) / zoomLevel
            );
        }

        public Point PdfToScreen(PointF pdfPoint, float zoomLevel, PointF documentOffset)
        {
            return new Point(
                (int)(pdfPoint.X * zoomLevel + documentOffset.X),
                (int)(pdfPoint.Y * zoomLevel + documentOffset.Y)
            );
        }

        public RectangleF ScreenToPdf(Rectangle screenRect, float zoomLevel, PointF documentOffset)
        {
            var topLeft = ScreenToPdf(new Point(screenRect.Left, screenRect.Top), zoomLevel, documentOffset);
            var bottomRight = ScreenToPdf(new Point(screenRect.Right, screenRect.Bottom), zoomLevel, documentOffset);

            return new RectangleF(
                topLeft.X,
                topLeft.Y,
                bottomRight.X - topLeft.X,
                bottomRight.Y - topLeft.Y
            );
        }

        public Rectangle PdfToScreen(RectangleF pdfRect, float zoomLevel, PointF documentOffset)
        {
            var topLeft = PdfToScreen(new PointF(pdfRect.Left, pdfRect.Top), zoomLevel, documentOffset);
            var bottomRight = PdfToScreen(new PointF(pdfRect.Right, pdfRect.Bottom), zoomLevel, documentOffset);

            return new Rectangle(
                topLeft.X,
                topLeft.Y,
                bottomRight.X - topLeft.X,
                bottomRight.Y - topLeft.Y
            );
        }

        public bool IsPointInElement(PointF point, PdfElement element)
        {
            return point.X >= element.X && point.X <= element.X + element.Width &&
                   point.Y >= element.Y && point.Y <= element.Y + element.Height;
        }

        public PointF CalculateCenterPoint(PdfElement element)
        {
            return new PointF(
                (float)(element.X + element.Width / 2),
                (float)(element.Y + element.Height / 2)
            );
        }

        public PointF CalculateRelativePosition(PointF absolutePoint, PdfElement parentElement)
        {
            return new PointF(
                absolutePoint.X - (float)parentElement.X,
                absolutePoint.Y - (float)parentElement.Y
            );
        }
    }
}