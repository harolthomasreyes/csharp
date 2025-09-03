// MyPdfEditor.WPF/Views/DocumentView.xaml.cs
using MyPdfEditor.Core.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPdfEditor.WPF.Views
{
    public partial class DocumentView : UserControl
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                DataContext is ViewModels.DocumentViewModel vm &&
                vm.SelectedField != null)
            {
                // Handle drag operation
                var position = e.GetPosition(Canvas);
                var pdfPoint = CoordinateConverter.ScreenToPdf(
                    position.X, position.Y,
                    vm.CurrentPage?.Page,
                    vm.CurrentPage?.ZoomLevel ?? 100);

                // Update field position through ViewModel
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ViewModels.DocumentViewModel vm)
            {
                var position = e.GetPosition(Canvas);
                var hitTest = VisualTreeHelper.HitTest(Canvas, position);

                if (hitTest != null)
                {
                    // Find the field that was clicked
                    var fieldElement = FindParentFieldElement(hitTest.VisualHit);
                    if (fieldElement != null && fieldElement.DataContext is ViewModels.FormFieldViewModel fieldVm)
                    {
                        vm.SelectField(fieldVm);
                        e.Handled = true;
                    }
                    else
                    {
                        vm.ClearSelection();
                    }
                }
            }
        }

        private FrameworkElement FindParentFieldElement(DependencyObject element)
        {
            while (element != null && !(element is FrameworkElement fe && fe.DataContext is ViewModels.FormFieldViewModel))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element as FrameworkElement;
        }
    }
}