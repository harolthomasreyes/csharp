// MyPdfEditor.WPF/Behaviors/DragDropBehavior.cs
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class DragDropBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(DragDropBehavior), new PropertyMetadata(true));

        public static readonly DependencyProperty DragCommandProperty =
            DependencyProperty.Register(nameof(DragCommand), typeof(ICommand), typeof(DragDropBehavior));

        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.Register(nameof(DropCommand), typeof(ICommand), typeof(DragDropBehavior));

        private bool _isDragging;
        private Point _startPoint;
        private object _dragData;

        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        public ICommand DragCommand
        {
            get => (ICommand)GetValue(DragCommandProperty);
            set => SetValue(DragCommandProperty, value);
        }

        public ICommand DropCommand
        {
            get => (ICommand)GetValue(DropCommandProperty);
            set => SetValue(DropCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.AllowDrop = true;

            AssociatedObject.DragEnter += OnDragEnter;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.DragLeave += OnDragLeave;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;

            AssociatedObject.DragEnter -= OnDragEnter;
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.DragLeave -= OnDragLeave;
            AssociatedObject.Drop -= OnDrop;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsDraggable || e.ChangedButton != MouseButton.Left)
                return;

            _startPoint = e.GetPosition(AssociatedObject);
            _isDragging = false;
            AssociatedObject.CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDraggable || !AssociatedObject.IsMouseCaptured || e.LeftButton != MouseButtonState.Pressed)
                return;

            var currentPoint = e.GetPosition(AssociatedObject);
            var diff = _startPoint - currentPoint;

            if (!_isDragging && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                StartDrag();
            }

            if (_isDragging)
            {
                e.Handled = true;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AssociatedObject.IsMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
                _isDragging = false;
            }
        }

        private void StartDrag()
        {
            _isDragging = true;
            _dragData = AssociatedObject.DataContext;

            if (DragCommand?.CanExecute(_dragData) == true)
            {
                DragCommand.Execute(_dragData);
            }

            var dataObject = new DataObject("FieldData", _dragData);
            DragDrop.DoDragDrop(AssociatedObject, dataObject, DragDropEffects.Move);

            _isDragging = false;
            AssociatedObject.ReleaseMouseCapture();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("FieldData"))
                return;

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("FieldData"))
                return;

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("FieldData"))
                return;

            var dropData = e.Data.GetData("FieldData");
            var dropPosition = e.GetPosition(AssociatedObject);

            if (DropCommand?.CanExecute(new DropParameters(dropData, dropPosition)) == true)
            {
                DropCommand.Execute(new DropParameters(dropData, dropPosition));
            }

            e.Handled = true;
        }
    }

    public class DropParameters
    {
        public object Data { get; }
        public Point Position { get; }

        public DropParameters(object data, Point position)
        {
            Data = data;
            Position = position;
        }
    }
}