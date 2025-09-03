// MyPdfEditor.WPF/Behaviors/ResizeBehavior.cs
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public enum ResizeHandlePosition
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
        None
    }

    public class ResizeBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.Register(nameof(IsResizable), typeof(bool), typeof(ResizeBehavior), new PropertyMetadata(true));

        public static readonly DependencyProperty ResizeCommandProperty =
            DependencyProperty.Register(nameof(ResizeCommand), typeof(ICommand), typeof(ResizeBehavior));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(nameof(MinWidth), typeof(double), typeof(ResizeBehavior), new PropertyMetadata(10.0));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(nameof(MinHeight), typeof(double), typeof(ResizeBehavior), new PropertyMetadata(10.0));

        public static readonly DependencyProperty HandleSizeProperty =
            DependencyProperty.Register(nameof(HandleSize), typeof(double), typeof(ResizeBehavior), new PropertyMetadata(8.0));

        private ResizeHandlePosition _activeHandle = ResizeHandlePosition.None;
        private Point _startPoint;
        private Rect _startRect;
        private bool _isResizing;

        public bool IsResizable
        {
            get => (bool)GetValue(IsResizableProperty);
            set => SetValue(IsResizableProperty, value);
        }

        public ICommand ResizeCommand
        {
            get => (ICommand)GetValue(ResizeCommandProperty);
            set => SetValue(ResizeCommandProperty, value);
        }

        public double MinWidth
        {
            get => (double)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        public double MinHeight
        {
            get => (double)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }

        public double HandleSize
        {
            get => (double)GetValue(HandleSizeProperty);
            set => SetValue(HandleSizeProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsResizable)
                return;

            if (_isResizing)
            {
                ContinueResize(e);
            }
            else
            {
                UpdateCursor(e.GetPosition(AssociatedObject));
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsResizable || _activeHandle == ResizeHandlePosition.None)
                return;

            _isResizing = true;
            _startPoint = e.GetPosition(AssociatedObject);
            _startRect = new Rect(0, 0, AssociatedObject.ActualWidth, AssociatedObject.ActualHeight);
            AssociatedObject.CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isResizing)
            {
                _isResizing = false;
                AssociatedObject.ReleaseMouseCapture();
                UpdateCursor(e.GetPosition(AssociatedObject));
                e.Handled = true;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isResizing)
            {
                _activeHandle = ResizeHandlePosition.None;
                AssociatedObject.Cursor = Cursors.Arrow;
            }
        }

        private void UpdateCursor(Point position)
        {
            var handle = GetHandleAtPosition(position);

            _activeHandle = handle;
            AssociatedObject.Cursor = GetCursorForHandle(handle);
        }

        private ResizeHandlePosition GetHandleAtPosition(Point position)
        {
            var width = AssociatedObject.ActualWidth;
            var height = AssociatedObject.ActualHeight;

            if (position.X < HandleSize)
            {
                if (position.Y < HandleSize) return ResizeHandlePosition.TopLeft;
                if (position.Y > height - HandleSize) return ResizeHandlePosition.BottomLeft;
                return ResizeHandlePosition.Left;
            }

            if (position.X > width - HandleSize)
            {
                if (position.Y < HandleSize) return ResizeHandlePosition.TopRight;
                if (position.Y > height - HandleSize) return ResizeHandlePosition.BottomRight;
                return ResizeHandlePosition.Right;
            }

            if (position.Y < HandleSize) return ResizeHandlePosition.Top;
            if (position.Y > height - HandleSize) return ResizeHandlePosition.Bottom;

            return ResizeHandlePosition.None;
        }

        private Cursor GetCursorForHandle(ResizeHandlePosition handle)
        {
            return handle switch
            {
                ResizeHandlePosition.TopLeft => Cursors.SizeNWSE,
                ResizeHandlePosition.Top => Cursors.SizeNS,
                ResizeHandlePosition.TopRight => Cursors.SizeNESW,
                ResizeHandlePosition.Left => Cursors.SizeWE,
                ResizeHandlePosition.Right => Cursors.SizeWE,
                ResizeHandlePosition.BottomLeft => Cursors.SizeNESW,
                ResizeHandlePosition.Bottom => Cursors.SizeNS,
                ResizeHandlePosition.BottomRight => Cursors.SizeNWSE,
                _ => Cursors.Arrow
            };
        }

        private void ContinueResize(MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(AssociatedObject);
            var deltaX = currentPoint.X - _startPoint.X;
            var deltaY = currentPoint.Y - _startPoint.Y;

            var newRect = CalculateNewRect(deltaX, deltaY);

            if (ResizeCommand?.CanExecute(new ResizeParameters(newRect)) == true)
            {
                ResizeCommand.Execute(new ResizeParameters(newRect));
            }
        }

        private Rect CalculateNewRect(double deltaX, double deltaY)
        {
            var newRect = _startRect;

            switch (_activeHandle)
            {
                case ResizeHandlePosition.TopLeft:
                    newRect.X += deltaX;
                    newRect.Y += deltaY;
                    newRect.Width = Math.Max(MinWidth, newRect.Width - deltaX);
                    newRect.Height = Math.Max(MinHeight, newRect.Height - deltaY);
                    break;

                case ResizeHandlePosition.Top:
                    newRect.Y += deltaY;
                    newRect.Height = Math.Max(MinHeight, newRect.Height - deltaY);
                    break;

                case ResizeHandlePosition.TopRight:
                    newRect.Y += deltaY;
                    newRect.Width = Math.Max(MinWidth, newRect.Width + deltaX);
                    newRect.Height = Math.Max(MinHeight, newRect.Height - deltaY);
                    break;

                case ResizeHandlePosition.Left:
                    newRect.X += deltaX;
                    newRect.Width = Math.Max(MinWidth, newRect.Width - deltaX);
                    break;

                case ResizeHandlePosition.Right:
                    newRect.Width = Math.Max(MinWidth, newRect.Width + deltaX);
                    break;

                case ResizeHandlePosition.BottomLeft:
                    newRect.X += deltaX;
                    newRect.Width = Math.Max(MinWidth, newRect.Width - deltaX);
                    newRect.Height = Math.Max(MinHeight, newRect.Height + deltaY);
                    break;

                case ResizeHandlePosition.Bottom:
                    newRect.Height = Math.Max(MinHeight, newRect.Height + deltaY);
                    break;

                case ResizeHandlePosition.BottomRight:
                    newRect.Width = Math.Max(MinWidth, newRect.Width + deltaX);
                    newRect.Height = Math.Max(MinHeight, newRect.Height + deltaY);
                    break;
            }

            return newRect;
        }
    }

    public class ResizeParameters
    {
        public Rect NewBounds { get; }

        public ResizeParameters(Rect newBounds)
        {
            NewBounds = newBounds;
        }
    }
}