using Microsoft.Win32;
using MyPdfEditor.MyPdfEditor.WPF.Views.Dialogs;
using MyPdfEditor.WPF.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPdfEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                // Load recent documents or perform other initialization
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("My PDF Editor\nVersion 1.0\n\nA powerful PDF editor with form creation capabilities.",
                "About My PDF Editor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OpenDocumentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                Title = "Open PDF Document",
                CheckFileExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (DataContext is MainViewModel vm)
                {
                    await vm.OpenDocumentAsync();//openFileDialog.FileName);
                }
            }
        }

        private async void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                Title = "Save PDF Document",
                DefaultExt = ".pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (DataContext is MainViewModel vm && vm.CurrentDocument != null)
                {
                    await vm.SaveDocumentAsync();//vm.CurrentDocument, saveFileDialog.FileName, true);
                }
            }
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewDocumentDialog
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dialog.ShowDialog() == true)
            {
                if (DataContext is MainViewModel vm)
                {
                    // Handle new document creation with dialog results
                }
            }
        }

        private void SecuritySettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.CurrentDocument != null)
            {
                var dialog = new SecurityDialog
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = vm.CurrentDocument.Security
                };

                dialog.ShowDialog();
            }
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.CurrentDocument != null)
            {
                var dialog = new ExportDialog
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dialog.ShowDialog();
            }
        }

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (DataContext is MainViewModel vm)
                {
                    if (e.Delta > 0)
                    {
                        vm.ZoomInCommand.Execute(null);
                    }
                    else
                    {
                        vm.ZoomOutCommand.Execute(null);
                    }
                    e.Handled = true;
                }
            }
        }

        private void MainScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            // Update scroll position for any necessary calculations
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.CurrentDocument?.IsModified == true)
            {
                var result = MessageBox.Show("Do you want to save changes before exiting?",
                    "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(vm.CurrentDocument.FilePath))
                    {
                        SaveAsMenuItem_Click(sender, null);
                    }
                    else
                    {
                        vm.SaveDocumentCommand.Execute(null);
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Implement cut functionality
            if (DataContext is MainViewModel vm && vm.DocumentViewModel?.SelectedField != null)
            {
                // Handle field cutting
            }
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Implement copy functionality
            if (DataContext is MainViewModel vm && vm.DocumentViewModel?.SelectedField != null)
            {
                // Handle field copying
            }
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Implement paste functionality
            if (DataContext is MainViewModel vm)
            {
                // Handle field pasting
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.DocumentViewModel?.SelectedField != null)
            {
                vm.DocumentViewModel.DeleteSelectedFieldCommand.Execute(null);
            }
        }

        private void AddTextFieldMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DocumentViewModel?.AddTextFieldCommand.Execute(null);
            }
        }

        private void AddCheckboxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DocumentViewModel?.AddCheckboxCommand.Execute(null);
            }
        }

        private void AddRadioButtonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DocumentViewModel?.AddRadioButtonCommand.Execute(null);
            }
        }

        private void AddComboBoxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DocumentViewModel?.AddComboBoxCommand.Execute(null);
            }
        }

        private void AddListBoxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DocumentViewModel?.AddListBoxCommand.Execute(null);
            }
        }

        private void AddButtonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.DocumentViewModel?.AddButtonCommand.Execute(null);
            }
        }

        private void ViewMenu_DropDownOpened(object sender, RoutedEventArgs e)
        {
            // Update menu item states based on current view
            if (DataContext is MainViewModel vm)
            {
                // Update zoom menu items, etc.
            }
        }

        private void ToolsMenu_DropDownOpened(object sender, RoutedEventArgs e)
        {
            // Update tools menu based on current selection
            if (DataContext is MainViewModel vm)
            {
                // Enable/disable tools based on document state
            }
        }
    }
}