// MyPdfEditor.WPF/ViewModels/PdfPageViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using MyPdfEditor.Core.Models.Document;
using MyPdfEditor.Core.Models.Forms;
using System.Collections.ObjectModel;

namespace MyPdfEditor.WPF.ViewModels
{
    public partial class PdfPageViewModel : ObservableObject
    {
        private readonly PdfPage _page;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private int _pageNumber;

        public ObservableCollection<FormFieldViewModel> Fields { get; } = new ObservableCollection<FormFieldViewModel>();

        public PdfPage Page => _page;

        public PdfPageViewModel(PdfPage page)
        {
            _page = page;
            Width = page.Width;
            Height = page.Height;
            PageNumber = page.PageNumber;

            InitializeFields();
        }

        private void InitializeFields()
        {
            Fields.Clear();
            foreach (var field in _page.FormFields)
            {
                Fields.Add(CreateFieldViewModel(field));
            }
        }

        public void AddField(FormField field)
        {
            _page.AddField(field);
            Fields.Add(CreateFieldViewModel(field));
        }

        public void RemoveField(FormField field)
        {
            _page.RemoveField(field);
            var viewModel = Fields.FirstOrDefault(f => f.Model == field);
            if (viewModel != null)
            {
                Fields.Remove(viewModel);
            }
        }

        private FormFieldViewModel CreateFieldViewModel(FormField field)
        {
            return field switch
            {
                TextField textField => new TextFieldViewModel(textField),
                CheckboxField checkbox => new CheckboxFieldViewModel(checkbox),
                //RadioButtonField radio => new RadioButtonFieldViewModel(radio),
                //ComboBoxField combo => new ComboBoxFieldViewModel(combo),
                //ListBoxField list => new ListBoxFieldViewModel(list),
                //ButtonField button => new ButtonFieldViewModel(button),
                _ => new FormFieldViewModel(field)
            };
        }
    }
}