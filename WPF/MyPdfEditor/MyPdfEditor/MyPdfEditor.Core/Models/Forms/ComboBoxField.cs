using MyPdfEditor.Core.Models.Enums;

namespace MyPdfEditor.Core.Models.Forms
{
    public class ComboBoxField : FormField
    {
        private List<string> _items = new List<string>();
        private string _selectedItem;

        public ComboBoxField()
        {
            FieldType = FieldType.ComboBox;
            Height = 25;
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

        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value && (value == null || Items.Contains(value)))
                {
                    _selectedItem = value;
                    OnValueChanged();
                }
            }
        }

        public bool IsEditable { get; set; }
        public int SelectedIndex => Items.IndexOf(SelectedItem);

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
                if (SelectedItem == item)
                {
                    SelectedItem = Items.FirstOrDefault();
                }
                OnItemsChanged();
            }
        }

        public void ClearItems()
        {
            Items.Clear();
            SelectedItem = null;
            OnItemsChanged();
        }

        public override object GetValue() => SelectedItem;

        public override void SetValue(object value)
        {
            if (value is string stringValue && Items.Contains(stringValue))
            {
                SelectedItem = stringValue;
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
}