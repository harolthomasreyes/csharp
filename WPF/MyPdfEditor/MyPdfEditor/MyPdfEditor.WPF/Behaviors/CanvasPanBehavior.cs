// MyPdfEditor.WPF/Behaviors/CanvasPanBehavior.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class CanvasPanBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register(nameof(IsPanEnabled), typeof(bool), typeof(CanvasPanBehavior),
                new PropertyMetadata(true));

        public static readonly DependencyProperty PanCommandProperty =
            DependencyProperty.Register(nameof(PanCommand), typeof(ICommand), typeof(CanvasPanBehavior));

        private Point _startPoint;
        private bool _isPanning;

        public bool IsPanEnabled
        {
            get => (bool)GetValue(IsPanEnabledProperty);
            set => SetValue(IsPanEnabledProperty, value);
        }

        public ICommand PanCommand
        {
            get => (ICommand)GetValue(PanCommandProperty);
            set => SetValue(PanCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseRightButtonDown += OnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp += OnMouseRightButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseRightButtonDown -= OnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp -= OnMouseRightButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsPanEnabled) return;

            _isPanning = true;
            _startPoint = e.GetPosition(AssociatedObject);
            AssociatedObject.CaptureMouse();
            AssociatedObject.Cursor = Cursors.ScrollAll;
            e.Handled = true;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                AssociatedObject.ReleaseMouseCapture();
                AssociatedObject.Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isPanning) return;

            var currentPoint = e.GetPosition(AssociatedObject);
            var deltaX = currentPoint.X - _startPoint.X;
            var deltaY = currentPoint.Y - _startPoint.Y;

            if (PanCommand?.CanExecute(new PanParameters(deltaX, deltaY)) == true)
            {
                PanCommand.Execute(new PanParameters(deltaX, deltaY));
            }

            _startPoint = currentPoint;
            e.Handled = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                AssociatedObject.ReleaseMouseCapture();
                AssociatedObject.Cursor = Cursors.Arrow;
            }
        }
    }

    public class PanParameters
    {
        public double DeltaX { get; }
        public double DeltaY { get; }

        public PanParameters(double deltaX, double deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }
}