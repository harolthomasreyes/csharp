// MyPdfEditor.WPF/Behaviors/ZoomBehavior.cs
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class ZoomBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ZoomCommandProperty =
            DependencyProperty.Register(nameof(ZoomCommand), typeof(ICommand), typeof(ZoomBehavior));

        public static readonly DependencyProperty ZoomSensitivityProperty =
            DependencyProperty.Register(nameof(ZoomSensitivity), typeof(double), typeof(ZoomBehavior), new PropertyMetadata(0.1));

        private Point _lastMousePosition;

        public ICommand ZoomCommand
        {
            get => (ICommand)GetValue(ZoomCommandProperty);
            set => SetValue(ZoomCommandProperty, value);
        }

        public double ZoomSensitivity
        {
            get => (double)GetValue(ZoomSensitivityProperty);
            set => SetValue(ZoomSensitivityProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseWheel += OnMouseWheel;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseWheel -= OnMouseWheel;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ZoomCommand == null || !e.Handled)
                return;

            var delta = e.Delta > 0 ? ZoomSensitivity : -ZoomSensitivity;
            var zoomCenter = _lastMousePosition;

            if (ZoomCommand.CanExecute(new ZoomParameters(delta, zoomCenter)))
            {
                ZoomCommand.Execute(new ZoomParameters(delta, zoomCenter));
                e.Handled = true;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _lastMousePosition = e.GetPosition(AssociatedObject);
        }
    }

    public class ZoomParameters
    {
        public double Delta { get; }
        public Point Center { get; }

        public ZoomParameters(double delta, Point center)
        {
            Delta = delta;
            Center = center;
        }
    }
}