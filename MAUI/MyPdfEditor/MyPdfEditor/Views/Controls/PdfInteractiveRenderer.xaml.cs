using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPdfEditor.Views.Controls
{
    /// <summary>
    /// Interaction logic for PdfInteractiveRenderer.xaml
    /// </summary>
    public partial class PdfInteractiveRenderer : Window
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PdfViewModel), typeof(PdfInteractiveRenderer),
                new PropertyMetadata(null, OnViewModelChanged));

        public PdfViewModel ViewModel
        {
            get => (PdfViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private readonly Dictionary<string, FrameworkElement> _visualElements = new();
        private ScaleTransform _currentTransform = new();

        public PdfInteractiveRenderer()
        {
            InitializeComponent();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            InteractiveCanvas.MouseLeftButtonDown += OnCanvasMouseDown;
            InteractiveCanvas.MouseMove += OnCanvasMouseMove;
            InteractiveCanvas.MouseLeftButtonUp += OnCanvasMouseUp;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SubscribeToViewModelEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnsubscribeFromViewModelEvents();
        }

        private void SubscribeToViewModelEvents()
        {
            if (ViewModel == null) return;

            ViewModel.ZoomChanged += OnZoomChanged;
            ViewModel.ElementsChanged += OnElementsChanged;
            ViewModel.SelectionUpdated += OnSelectionUpdated;
        }

        private void UnsubscribeFromViewModelEvents()
        {
            if (ViewModel == null) return;

            ViewModel.ZoomChanged -= OnZoomChanged;
            ViewModel.ElementsChanged -= OnElementsChanged;
            ViewModel.SelectionUpdated -= OnSelectionUpdated;
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PdfInteractiveRenderer renderer)
            {
                renderer.UnsubscribeFromViewModelEvents();
                renderer.SubscribeToViewModelEvents();
                renderer.RenderVisualElements();
            }
        }

        private void OnZoomChanged(object sender, double zoomLevel)
        {
            UpdateTransform(zoomLevel);
            UpdateElementPositions();
        }

        private void OnElementsChanged(object sender, EventArgs e)
        {
            RenderVisualElements();
        }

        private void OnSelectionUpdated(object sender, EventArgs e)
        {
            UpdateSelectionVisuals();
        }

        private void UpdateTransform(double zoomLevel)
        {
            _currentTransform = new ScaleTransform(zoomLevel, zoomLevel);
            foreach (var element in _visualElements.Values)
            {
                element.RenderTransform = _currentTransform;
            }
        }

        private void RenderVisualElements()
        {
            InteractiveCanvas.Children.Clear();
            _visualElements.Clear();

            if (ViewModel?.Elements == null) return;

            foreach (var element in ViewModel.Elements)
            {
                var visualElement = CreateVisualElement(element);
                if (visualElement != null)
                {
                    _visualElements[element.Id] = visualElement;
                    InteractiveCanvas.Children.Add(visualElement);
                }
            }
        }

        private FrameworkElement CreateVisualElement(PdfElementViewModel element)
        {
            return element.Type switch
            {
                ElementType.TextField => CreateTextField(element),
                ElementType.Checkbox => CreateCheckbox(element),
                ElementType.RadioButton => CreateRadioButton(element),
                _ => null
            };
        }

        private FrameworkElement CreateTextField(PdfElementViewModel element)
        {
            var border = new Border
            {
                Width = element.Width,
                Height = element.Height,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Tag = element.Id
            };

            Canvas.SetLeft(border, element.X);
            Canvas.SetTop(border, element.Y);

            var textBlock = new TextBlock
            {
                Text = element.Text ?? "Text",
                Margin = new Thickness(4),
                Foreground = Brushes.Black,
                FontSize = 11,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = textBlock;
            return border;
        }

        private FrameworkElement CreateCheckbox(PdfElementViewModel element)
        {
            var box = new Border
            {
                Width = 16,
                Height = 16,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Tag = element.Id
            };

            if (element.IsChecked)
            {
                box.Child = new TextBlock
                {
                    Text = "✓",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeight.Bold
                };
            }

            Canvas.SetLeft(box, element.X);
            Canvas.SetTop(box, element.Y);
            return box;
        }

        private FrameworkElement CreateRadioButton(PdfElementViewModel element)
        {
            var ellipse = new Ellipse
            {
                Width = 16,
                Height = 16,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = element.IsChecked ? Brushes.Black : Brushes.White,
                Tag = element.Id
            };

            Canvas.SetLeft(ellipse, element.X);
            Canvas.SetTop(ellipse, element.Y);
            return ellipse;
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Selection logic placeholder
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            // Drag logic placeholder
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Operation completion placeholder
        }

        private void UpdateElementPositions()
        {
            // Position update logic
        }

        private void UpdateSelectionVisuals()
        {
            // Selection visual update logic
        }
    }
}
