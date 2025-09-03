// MyPdfEditor.WPF/ViewModels/PropertiesPanelViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPdfEditor.Core.Models.Enums;
using MyPdfEditor.Core.Models.Forms;
using MyPdfEditor.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class PropertiesPanelViewModel : ObservableObject
    {
        [ObservableProperty]
        private FieldViewModel _selectedField;

        [ObservableProperty]
        private bool _hasSelection;

        [ObservableProperty]
        private bool _isMultipleSelection;

        [ObservableProperty]
        private int _selectionCount;

        [ObservableProperty]
        private string _panelTitle = "Properties";

        [ObservableProperty]
        private bool _isTextPropertiesVisible;

        [ObservableProperty]
        private bool _isCheckboxPropertiesVisible;

        [ObservableProperty]
        private bool _isRadioButtonPropertiesVisible;

        [ObservableProperty]
        private bool _isComboBoxPropertiesVisible;

        [ObservableProperty]
        private bool _isListBoxPropertiesVisible;

        [ObservableProperty]
        private bool _isButtonPropertiesVisible;

        // Common properties
        [ObservableProperty]
        private string _fieldName;

        [ObservableProperty]
        private double _positionX;

        [ObservableProperty]
        private double _positionY;

        [ObservableProperty]
        private double _fieldWidth;

        [ObservableProperty]
        private double _fieldHeight;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private bool _isReadOnly;

        [ObservableProperty]
        private bool _isRequired;

        [ObservableProperty]
        private string _validationPattern;

        [ObservableProperty]
        private string _borderColor;

        [ObservableProperty]
        private double _borderWidth;

        [ObservableProperty]
        private string _backgroundColor;

        [ObservableProperty]
        private string _toolTip;

        // Text field properties
        [ObservableProperty]
        private string _textValue;

        [ObservableProperty]
        private string _fontFamily;

        [ObservableProperty]
        private double _fontSize;

        [ObservableProperty]
        private string _fontColor;

        [ObservableProperty]
        private bool _isMultiline;

        [ObservableProperty]
        private int _maxLength;

        [ObservableProperty]
        private TextAlignment _textAlignment;

        // Checkbox properties
        [ObservableProperty]
        private bool _isChecked;

        [ObservableProperty]
        private string _checkedSymbol;

        [ObservableProperty]
        private string _checkMarkColor;

        // Radio button properties
        [ObservableProperty]
        private string _groupName;

        [ObservableProperty]
        private string _selectedSymbol;

        [ObservableProperty]
        private string _selectionColor;

        // ComboBox properties
        [ObservableProperty]
        private ObservableCollection<string> _comboItems = new ObservableCollection<string>();

        [ObservableProperty]
        private string _selectedComboItem;

        [ObservableProperty]
        private bool _isEditable;

        [ObservableProperty]
        private string _newItemText;

        // ListBox properties
        [ObservableProperty]
        private ObservableCollection<string> _listItems = new ObservableCollection<string>();

        [ObservableProperty]
        private ObservableCollection<string> _selectedListItems = new ObservableCollection<string>();

        [ObservableProperty]
        private SelectionMode _selectionMode;

        // Button properties
        [ObservableProperty]
        private string _buttonCaption;

        [ObservableProperty]
        private string _buttonFontFamily;

        [ObservableProperty]
        private double _buttonFontSize;

        [ObservableProperty]
        private string _buttonFontColor;

        [ObservableProperty]
        private ButtonAction _buttonAction;

        [ObservableProperty]
        private string _actionParameter;

        // Available options
        public ObservableCollection<string> AvailableFonts { get; } = new ObservableCollection<string>
        {
            "Arial", "Times New Roman", "Courier New", "Verdana", "Tahoma", "Calibri"
        };

        public ObservableCollection<double> FontSizes { get; } = new ObservableCollection<double>
        {
            8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72
        };

        public ObservableCollection<TextAlignment> TextAlignments { get; } = new ObservableCollection<TextAlignment>
        {
            TextAlignment.Left, TextAlignment.Center, TextAlignment.Right, TextAlignment.Justified
        };

        public ObservableCollection<ButtonAction> ButtonActions { get; } = new ObservableCollection<ButtonAction>
        {
            ButtonAction.None, ButtonAction.SubmitForm, ButtonAction.ResetForm,
            ButtonAction.PrintDocument, ButtonAction.OpenUrl, ButtonAction.ExecuteScript, ButtonAction.Custom
        };

        public ObservableCollection<SelectionMode> SelectionModes { get; } = new ObservableCollection<SelectionMode>
        {
            SelectionMode.Single, SelectionMode.Multiple
        };

        public ICommand AddComboItemCommand { get; }
        public ICommand RemoveComboItemCommand { get; }
        public ICommand AddListItemCommand { get; }
        public ICommand RemoveListItemCommand { get; }
        public ICommand MoveItemUpCommand { get; }
        public ICommand MoveItemDownCommand { get; }

        public PropertiesPanelViewModel()
        {
            AddComboItemCommand = new RelayCommand(AddComboItem, () => !string.IsNullOrWhiteSpace(NewItemText));
            RemoveComboItemCommand = new RelayCommand(RemoveSelectedComboItem, () => !string.IsNullOrEmpty(SelectedComboItem));
            AddListItemCommand = new RelayCommand(AddListItem, () => !string.IsNullOrWhiteSpace(NewItemText));
            RemoveListItemCommand = new RelayCommand(RemoveSelectedListItem, () => SelectedListItems.Count > 0);
            MoveItemUpCommand = new RelayCommand(MoveItemUp, () => CanMoveItemUp());
            MoveItemDownCommand = new RelayCommand(MoveItemDown, () => CanMoveItemDown());
        }

        public void UpdateSelection(FieldViewModel field)
        {
            SelectedField = field;
            HasSelection = field != null;
            IsMultipleSelection = false;
            SelectionCount = HasSelection ? 1 : 0;

            if (field != null)
            {
                UpdatePropertiesFromField(field);
            }
            else
            {
                ResetProperties();
            }
        }

        public void UpdateMultipleSelection(IEnumerable<FieldViewModel> fields)
        {
            var fieldList = fields?.ToList();
            HasSelection = fieldList?.Count > 0;
            IsMultipleSelection = fieldList?.Count > 1;
            SelectionCount = fieldList?.Count ?? 0;

            if (fieldList?.Count == 1)
            {
                UpdateSelection(fieldList[0]);
            }
            else if (fieldList?.Count > 1)
            {
                UpdatePropertiesForMultipleSelection(fieldList);
            }
            else
            {
                ResetProperties();
            }
        }

        private void UpdatePropertiesFromField(FieldViewModel field)
        {
            // Common properties
            FieldName = field.Name;
            PositionX = field.X;
            PositionY = field.Y;
            FieldWidth = field.Width;
            FieldHeight = field.Height;
            IsVisible = field.IsVisible;
            IsReadOnly = field.IsReadOnly;
            IsRequired = field.IsRequired;
            ValidationPattern = field.ValidationPattern;
            BorderColor = field.BorderColor;
            BorderWidth = field.BorderWidth;
            BackgroundColor = field.BackgroundColor;
            ToolTip = field.ToolTip;

            // Hide all specific property sections initially
            ResetPropertySections();

            // Field type specific properties
            switch (field)
            {
                case TextFieldViewModel textField:
                    UpdateTextProperties(textField);
                    break;
                case CheckboxFieldViewModel checkbox:
                    UpdateCheckboxProperties(checkbox);
                    break;
                case RadioButtonFieldViewModel radio:
                    UpdateRadioButtonProperties(radio);
                    break;
                case ComboBoxFieldViewModel combo:
                    UpdateComboBoxProperties(combo);
                    break;
                case ListBoxFieldViewModel list:
                    UpdateListBoxProperties(list);
                    break;
                case ButtonFieldViewModel button:
                    UpdateButtonProperties(button);
                    break;
            }
        }

        private void UpdateTextProperties(TextFieldViewModel textField)
        {
            IsTextPropertiesVisible = true;
            TextValue = textField.Text;
            FontFamily = textField.FontFamily;
            FontSize = textField.FontSize;
            FontColor = textField.FontColor;
            IsMultiline = textField.IsMultiline;
            MaxLength = textField.MaxLength;
            TextAlignment = textField.TextAlignment;
        }

        private void UpdateCheckboxProperties(CheckboxFieldViewModel checkbox)
        {
            IsCheckboxPropertiesVisible = true;
            IsChecked = checkbox.IsChecked;
            CheckedSymbol = checkbox.CheckedSymbol;
            CheckMarkColor = checkbox.CheckMarkColor;
        }

        private void UpdateRadioButtonProperties(RadioButtonFieldViewModel radio)
        {
            IsRadioButtonPropertiesVisible = true;
            GroupName = radio.GroupName;
            SelectedSymbol = radio.SelectedSymbol;
            SelectionColor = radio.SelectionColor;
        }

        private void UpdateComboBoxProperties(ComboBoxFieldViewModel combo)
        {
            IsComboBoxPropertiesVisible = true;
            ComboItems.Clear();
            foreach (var item in combo.Items)
            {
                ComboItems.Add(item);
            }
            SelectedComboItem = combo.SelectedItem;
            IsEditable = combo.IsEditable;
        }

        private void UpdateListBoxProperties(ListBoxFieldViewModel list)
        {
            IsListBoxPropertiesVisible = true;
            ListItems.Clear();
            foreach (var item in list.Items)
            {
                ListItems.Add(item);
            }
            SelectedListItems.Clear();
            foreach (var item in list.SelectedItems)
            {
                SelectedListItems.Add(item);
            }
            SelectionMode = list.SelectionMode;
        }

        private void UpdateButtonProperties(ButtonFieldViewModel button)
        {
            IsButtonPropertiesVisible = true;
            ButtonCaption = button.Caption;
            ButtonFontFamily = button.FontFamily;
            ButtonFontSize = button.FontSize;
            ButtonFontColor = button.FontColor;
            ButtonAction = button.Action;
            ActionParameter = button.ActionParameter;
        }

        private void UpdatePropertiesForMultipleSelection(List<FieldViewModel> fields)
        {
            PanelTitle = $"{fields.Count} items selected";

            // Show only common properties that are the same across all selected fields
            FieldName = fields.All(f => f.Name == fields[0].Name) ? fields[0].Name : "(Multiple values)";
            IsVisible = fields.All(f => f.IsVisible == fields[0].IsVisible) ? fields[0].IsVisible : true;
            IsReadOnly = fields.All(f => f.IsReadOnly == fields[0].IsReadOnly) ? fields[0].IsReadOnly : false;

            // Hide all specific property sections
            ResetPropertySections();
        }

        private void ResetProperties()
        {
            PanelTitle = "Properties";
            FieldName = string.Empty;
            PositionX = 0;
            PositionY = 0;
            FieldWidth = 0;
            FieldHeight = 0;
            IsVisible = true;
            IsReadOnly = false;
            IsRequired = false;
            ValidationPattern = string.Empty;
            BorderColor = "#000000";
            BorderWidth = 1;
            BackgroundColor = "#FFFFFF";
            ToolTip = string.Empty;

            ResetPropertySections();
        }

        private void ResetPropertySections()
        {
            IsTextPropertiesVisible = false;
            IsCheckboxPropertiesVisible = false;
            IsRadioButtonPropertiesVisible = false;
            IsComboBoxPropertiesVisible = false;
            IsListBoxPropertiesVisible = false;
            IsButtonPropertiesVisible = false;
        }

        // Command implementations
        private void AddComboItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItemText) && !ComboItems.Contains(NewItemText))
            {
                ComboItems.Add(NewItemText);
                NewItemText = string.Empty;
            }
        }

        private void RemoveSelectedComboItem()
        {
            if (!string.IsNullOrEmpty(SelectedComboItem))
            {
                ComboItems.Remove(SelectedComboItem);
            }
        }

        private void AddListItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItemText) && !ListItems.Contains(NewItemText))
            {
                ListItems.Add(NewItemText);
                NewItemText = string.Empty;
            }
        }

        private void RemoveSelectedListItem()
        {
            foreach (var item in SelectedListItems.ToList())
            {
                ListItems.Remove(item);
            }
            SelectedListItems.Clear();
        }

        private bool CanMoveItemUp() => SelectedComboItem != null && ComboItems.IndexOf(SelectedComboItem) > 0;
        private bool CanMoveItemDown() => SelectedComboItem != null && ComboItems.IndexOf(SelectedComboItem) < ComboItems.Count - 1;

        private void MoveItemUp()
        {
            if (CanMoveItemUp())
            {
                var index = ComboItems.IndexOf(SelectedComboItem);
                ComboItems.Move(index, index - 1);
            }
        }

        private void MoveItemDown()
        {
            if (CanMoveItemDown())
            {
                var index = ComboItems.IndexOf(SelectedComboItem);
                ComboItems.Move(index, index + 1);
            }
        }

        // Property change handlers to update the actual field
        partial void OnFieldNameChanged(string value)
        {
            if (SelectedField != null) SelectedField.Name = value;
        }

        partial void OnPositionXChanged(double value)
        {
            if (SelectedField != null) SelectedField.X = value;
        }

        partial void OnPositionYChanged(double value)
        {
            if (SelectedField != null) SelectedField.Y = value;
        }

        partial void OnFieldWidthChanged(double value)
        {
            if (SelectedField != null) SelectedField.Width = value;
        }

        partial void OnFieldHeightChanged(double value)
        {
            if (SelectedField != null) SelectedField.Height = value;
        }

        partial void OnIsVisibleChanged(bool value)
        {
            if (SelectedField != null) SelectedField.IsVisible = value;
        }

        partial void OnIsReadOnlyChanged(bool value)
        {
            if (SelectedField != null) SelectedField.IsReadOnly = value;
        }

        // Additional property change handlers would be added for specific field types
    }
}