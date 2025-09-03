// MyPdfEditor.Core/Services/Interfaces/ICoordinateService.cs
using System.Threading.Tasks;
using MyPdfEditor.Core.Models.Document;

namespace MyPdfEditor.Core.Services.Interfaces
{
    public interface ICoordinateService
    {
        // Coordinate conversion
        (double X, double Y) ScreenToPdfCoordinates(double screenX, double screenY, PdfPage page, double zoomLevel);
        (double X, double Y) PdfToScreenCoordinates(double pdfX, double pdfY, PdfPage page, double zoomLevel);

        // Unit conversion
        double PointsToPixels(double points, double dpi = 96.0);
        double PixelsToPoints(double pixels, double dpi = 96.0);
        double MillimetersToPoints(double millimeters);
        double PointsToMillimeters(double points);

        // Zoom and scaling
        double CalculateZoomFactor(double zoomPercentage);
        (double Width, double Height) GetScaledPageDimensions(PdfPage page, double zoomLevel);

        // Validation
        bool IsPointWithinPage(double x, double y, PdfPage page);
        bool IsRectangleWithinPage(double x, double y, double width, double height, PdfPage page);

        // Grid and snapping
        (double X, double Y) SnapToGrid(double x, double y, double gridSize);
        (double X, double Y) SnapToNearestElement(double x, double y, PdfPage page, double tolerance);

        // Collision detection
        bool CheckCollision(double x1, double y1, double width1, double height1,
                          double x2, double y2, double width2, double height2);
        IEnumerable<(double X, double Y)> GetSuggestedPositions(PdfPage page, double width, double height);
    }
}