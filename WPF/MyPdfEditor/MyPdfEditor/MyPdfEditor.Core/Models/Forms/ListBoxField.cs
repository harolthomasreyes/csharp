using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Forms
{
    public class ListBoxField : FormField
    {
        private List<string> _items = new List<string>();
        private List<string> _selectedItems = new List<string>();

        public ListBoxField()
        {
            FieldType = FieldType.ListBox;
            Height = 100;
        }

        public List<string> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<string>();
                OnItemsChanged();
            }
        }

        public List<string> SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value?.Where(item => Items.Contains(item)).ToList() ?? new List<string>();
                OnValueChanged();
            }
        }

        public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;

        public void AddItem(string item)
        {
            if (!string.IsNullOrEmpty(item) && !Items.Contains(item))
            {
                Items.Add(item);
                OnItemsChanged();
            }
        }

        public void RemoveItem(string item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                SelectedItems.Remove(item);
                OnItemsChanged();
            }
        }

        public void ClearItems()
        {
            Items.Clear();
            SelectedItems.Clear();
            OnItemsChanged();
        }

        public void SelectItem(string item)
        {
            if (Items.Contains(item) && !SelectedItems.Contains(item))
            {
                if (SelectionMode == SelectionMode.Single)
                {
                    SelectedItems.Clear();
                }
                SelectedItems.Add(item);
                OnValueChanged();
            }
        }

        public void DeselectItem(string item)
        {
            if (SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
                OnValueChanged();
            }
        }

        public override object GetValue() => SelectionMode == SelectionMode.Single ?
            SelectedItems.FirstOrDefault() :
            SelectedItems;

        public override void SetValue(object value)
        {
            if (value is string singleValue && Items.Contains(singleValue))
            {
                SelectedItems = new List<string> { singleValue };
            }
            else if (value is IEnumerable<string> multipleValues)
            {
                SelectedItems = multipleValues.Where(item => Items.Contains(item)).ToList();
            }
        }

        public event EventHandler ValueChanged;
        public event EventHandler ItemsChanged;

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnItemsChanged()
        {
            ItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum SelectionMode
    {
        Single,
        Multiple
    }
}