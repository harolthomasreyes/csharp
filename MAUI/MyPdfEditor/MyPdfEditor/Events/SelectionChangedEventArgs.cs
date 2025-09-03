namespace MyPdfEditor.Events
{
    public class SelectionChangedEventArgs : EventArgs
    {
        public string ElementId { get; }
        public bool IsSelected { get; }

        public SelectionChangedEventArgs(string elementId, bool isSelected)
        {
            ElementId = elementId;
            IsSelected = isSelected;
        }
    }
}
