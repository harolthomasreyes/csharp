// MyPdfEditor.Core/Utilities/CoordinateConverter.cs
using System;
using MyPdfEditor.Core.Models.Document;

namespace MyPdfEditor.Core.Utilities
{
    public static class CoordinateConverter
    {
        // DPI constants (standard screen DPI)
        public const double DefaultDpi = 96.0;
        public const double PointsPerInch = 72.0;

        // Convert screen coordinates to PDF points
        public static (double X, double Y) ScreenToPdf(double screenX, double screenY, PdfPage page, double zoomLevel, double dpi = DefaultDpi)
        {
            var zoomFactor = zoomLevel / 100.0;
            var pointsX = PixelsToPoints(screenX / zoomFactor, dpi);
            var pointsY = PixelsToPoints(screenY / zoomFactor, dpi);

            return (pointsX, pointsY);
        }

        // Convert PDF points to screen coordinates
        public static (double X, double Y) PdfToScreen(double pdfX, double pdfY, PdfPage page, double zoomLevel, double dpi = DefaultDpi)
        {
            var zoomFactor = zoomLevel / 100.0;
            var pixelsX = PointsToPixels(pdfX, dpi) * zoomFactor;
            var pixelsY = PointsToPixels(pdfY, dpi) * zoomFactor;

            return (pixelsX, pixelsY);
        }

        // Unit conversion methods
        public static double PixelsToPoints(double pixels, double dpi = DefaultDpi)
        {
            return pixels * (PointsPerInch / dpi);
        }

        public static double PointsToPixels(double points, double dpi = DefaultDpi)
        {
            return points * (dpi / PointsPerInch);
        }

        public static double MillimetersToPoints(double millimeters)
        {
            return millimeters * 2.83465; // 1 mm = 2.83465 points
        }

        public static double PointsToMillimeters(double points)
        {
            return points / 2.83465;
        }

        public static double InchesToPoints(double inches)
        {
            return inches * PointsPerInch;
        }

        public static double PointsToInches(double points)
        {
            return points / PointsPerInch;
        }

        // Zoom and scaling utilities
        public static double CalculateZoomFactor(double zoomPercentage)
        {
            return zoomPercentage / 100.0;
        }

        public static (double Width, double Height) GetScaledPageDimensions(PdfPage page, double zoomLevel)
        {
            var zoomFactor = CalculateZoomFactor(zoomLevel);
            var widthPx = PointsToPixels(page.Width) * zoomFactor;
            var heightPx = PointsToPixels(page.Height) * zoomFactor;

            return (widthPx, heightPx);
        }

        // Coordinate validation
        public static bool IsPointWithinPage(double x, double y, PdfPage page)
        {
            return x >= 0 && x <= page.Width && y >= 0 && y <= page.Height;
        }

        public static bool IsRectangleWithinPage(double x, double y, double width, double height, PdfPage page)
        {
            return x >= 0 && y >= 0 &&
                   x + width <= page.Width &&
                   y + height <= page.Height;
        }

        // Grid snapping utilities
        public static (double X, double Y) SnapToGrid(double x, double y, double gridSize)
        {
            var snappedX = Math.Round(x / gridSize) * gridSize;
            var snappedY = Math.Round(y / gridSize) * gridSize;

            return (snappedX, snappedY);
        }

        public static (double X, double Y) SnapToNearestElement(double x, double y, double tolerance, params (double X, double Y)[] referencePoints)
        {
            foreach (var point in referencePoints)
            {
                var distance = CalculateDistance(x, y, point.X, point.Y);
                if (distance <= tolerance)
                {
                    return point;
                }
            }

            return (x, y);
        }

        // Collision detection
        public static bool CheckCollision(double x1, double y1, double width1, double height1,
                                        double x2, double y2, double width2, double height2)
        {
            return x1 < x2 + width2 &&
                   x1 + width1 > x2 &&
                   y1 < y2 + height2 &&
                   y1 + height1 > y2;
        }

        public static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Alignment helpers
        public static bool IsAlignedHorizontally(double y1, double y2, double tolerance = 2.0)
        {
            return Math.Abs(y1 - y2) <= tolerance;
        }

        public static bool IsAlignedVertically(double x1, double x2, double tolerance = 2.0)
        {
            return Math.Abs(x1 - x2) <= tolerance;
        }

        // Center point calculation
        public static (double CenterX, double CenterY) GetCenterPoint(double x, double y, double width, double height)
        {
            return (x + width / 2, y + height / 2);
        }
    }
}