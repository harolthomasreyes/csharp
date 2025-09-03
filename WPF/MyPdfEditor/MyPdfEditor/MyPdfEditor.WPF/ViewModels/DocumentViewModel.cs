// MyPdfEditor.WPF/ViewModels/DocumentViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Forms;
using MyPdfEditor.Core.Services.Interfaces;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class DocumentViewModel : ObservableObject
    {
        private readonly PdfDocument _document;
        private readonly IPdfService _pdfService;
        private readonly IFormFieldService _formFieldService;

        [ObservableProperty]
        private int _currentPageNumber = 1;

        [ObservableProperty]
        private PdfPageViewModel _currentPage;

        [ObservableProperty]
        private FormFieldViewModel _selectedField;

        [ObservableProperty]
        private bool _isFieldSelected;

        public ObservableCollection<PdfPageViewModel> Pages { get; } = new ObservableCollection<PdfPageViewModel>();
        public ObservableCollection<FormFieldViewModel> AllFields { get; } = new ObservableCollection<FormFieldViewModel>();

        public ICommand AddPageCommand { get; }
        public ICommand RemovePageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand AddTextFieldCommand { get; }
        public ICommand AddCheckboxCommand { get; }
        public ICommand AddRadioButtonCommand { get; }
        public ICommand AddComboBoxCommand { get; }
        public ICommand AddListBoxCommand { get; }
        public ICommand AddButtonCommand { get; }
        public ICommand DeleteSelectedFieldCommand { get; }

        public DocumentViewModel(PdfDocument document, IPdfService pdfService, IFormFieldService formFieldService = null)
        {
            _document = document;
            _pdfService = pdfService;
            _formFieldService = formFieldService;

            InitializePages();

            AddPageCommand = new RelayCommand(AddPage);
            RemovePageCommand = new RelayCommand(RemoveCurrentPage, CanRemovePage);
            NextPageCommand = new RelayCommand(NextPage, CanGoNext);
            PreviousPageCommand = new RelayCommand(PreviousPage, CanGoPrevious);
            AddTextFieldCommand = new RelayCommand(() => AddField(FieldType.Text));
            AddCheckboxCommand = new RelayCommand(() => AddField(FieldType.Checkbox));
            AddRadioButtonCommand = new RelayCommand(() => AddField(FieldType.RadioButton));
            AddComboBoxCommand = new RelayCommand(() => AddField(FieldType.ComboBox));
            AddListBoxCommand = new RelayCommand(() => AddField(FieldType.ListBox));
            AddButtonCommand = new RelayCommand(() => AddField(FieldType.Button));
            DeleteSelectedFieldCommand = new RelayCommand(DeleteSelectedField, () => IsFieldSelected);
        }

        private void InitializePages()
        {
            Pages.Clear();
            foreach (var page in _document.Pages)
            {
                Pages.Add(new PdfPageViewModel(page));
            }

            if (Pages.Count > 0)
            {
                CurrentPage = Pages[0];
            }
        }

        partial void OnCurrentPageNumberChanged(int value)
        {
            if (value > 0 && value <= Pages.Count)
            {
                CurrentPage = Pages[value - 1];
            }
        }

        partial void OnSelectedFieldChanged(FormFieldViewModel value)
        {
            IsFieldSelected = value != null;
        }

        private void AddPage()
        {
            _pdfService.AddPageAsync(_document, 595, 842).Wait();
            var newPage = new PdfPageViewModel(_document.Pages[^1]);
            Pages.Add(newPage);
            CurrentPageNumber = Pages.Count;
        }

        private void RemoveCurrentPage()
        {
            if (CurrentPageNumber > 0 && CurrentPageNumber <= Pages.Count)
            {
                _pdfService.RemovePageAsync(_document, CurrentPageNumber).Wait();
                Pages.RemoveAt(CurrentPageNumber - 1);

                if (Pages.Count == 0)
                {
                    AddPage();
                }
                else
                {
                    CurrentPageNumber = Math.Min(CurrentPageNumber, Pages.Count);
                }
            }
        }

        private bool CanRemovePage() => Pages.Count > 1;

        private void NextPage()
        {
            if (CanGoNext())
            {
                CurrentPageNumber++;
            }
        }

        private void PreviousPage()
        {
            if (CanGoPrevious())
            {
                CurrentPageNumber--;
            }
        }

        private bool CanGoNext() => CurrentPageNumber < Pages.Count;
        private bool CanGoPrevious() => CurrentPageNumber > 1;

        private void AddField(FieldType fieldType)
        {
            // Default position (center of page)
            double x = CurrentPage.Width / 2 - 50;
            double y = CurrentPage.Height / 2 - 10;

            FormField field = fieldType switch
            {
                FieldType.Text => new TextField { X = x, Y = y, Width = 100, Height = 20 },
                FieldType.Checkbox => new CheckboxField { X = x, Y = y },
                FieldType.RadioButton => new RadioButtonField { X = x, Y = y, GroupName = "Group1" },
                FieldType.ComboBox => new ComboBoxField { X = x, Y = y, Width = 100, Height = 25 },
                FieldType.ListBox => new ListBoxField { X = x, Y = y, Width = 100, Height = 60 },
                FieldType.Button => new ButtonField { X = x, Y = y, Width = 80, Height = 30 },
                _ => throw new ArgumentException("Invalid field type")
            };

            field.PageNumber = CurrentPageNumber;
            field.Name = $"{fieldType}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            CurrentPage.AddField(field);
            _document.IsModified = true;
        }

        private void DeleteSelectedField()
        {
            if (SelectedField != null)
            {
                CurrentPage.RemoveField(SelectedField.Model);
                SelectedField = null;
                _document.IsModified = true;
            }
        }

        public void SelectField(FormFieldViewModel field)
        {
            SelectedField = field;
        }

        public void ClearSelection()
        {
            SelectedField = null;
        }
    }
}