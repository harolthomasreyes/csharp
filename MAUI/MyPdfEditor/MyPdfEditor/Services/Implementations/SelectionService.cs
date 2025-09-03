using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyPdfEditor.Services.Implementations
{
    public class SelectionService : ISelectionService
    {
        private readonly HashSet<string> _selectedElements = new();

        public IReadOnlyCollection<string> SelectedElements => _selectedElements.ToList().AsReadOnly();

        public event SelectionChangedEventHandler SelectionChanged;

        public void SelectElement(string elementId)
        {
            if (_selectedElements.Add(elementId))
            {
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(elementId, true));
            }
        }

        public void DeselectElement(string elementId)
        {
            if (_selectedElements.Remove(elementId))
            {
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(elementId, false));
            }
        }

        public void ClearSelection()
        {
            var removed = _selectedElements.ToList();
            _selectedElements.Clear();
            foreach (var elementId in removed)
            {
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(elementId, false));
            }
        }

        public bool IsSelected(string elementId)
        {
            return _selectedElements.Contains(elementId);
        }

        public void SelectMultiple(IEnumerable<string> elementIds)
        {
            foreach (var id in elementIds)
            {
                _selectedElements.Add(id);
            }
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(null, true));
        }
    }
}
