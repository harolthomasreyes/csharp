using System.Windows.Controls;

namespace MyPdfEditor.Services.Interfaces
{
    public interface ISelectionService
    {
        IReadOnlyCollection<string> SelectedElements { get; }
        event SelectionChangedEventHandler SelectionChanged;
        void SelectElement(string elementId);
        void DeselectElement(string elementId);
        void ClearSelection();
        bool IsSelected(string elementId);
        void SelectMultiple(IEnumerable<string> elementIds);
    }
}
