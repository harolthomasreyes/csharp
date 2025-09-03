// MyPdfEditor.WPF/Behaviors/GridSnapBehavior.cs
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class GridSnapBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsSnapEnabledProperty =
            DependencyProperty.Register(nameof(IsSnapEnabled), typeof(bool), typeof(GridSnapBehavior),
                new PropertyMetadata(true));

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(GridSnapBehavior),
                new PropertyMetadata(10.0));

        public static readonly DependencyProperty SnapCommandProperty =
            DependencyProperty.Register(nameof(SnapCommand), typeof(ICommand), typeof(GridSnapBehavior));

        public bool IsSnapEnabled
        {
            get => (bool)GetValue(IsSnapEnabledProperty);
            set => SetValue(IsSnapEnabledProperty, value);
        }

        public double GridSize
        {
            get => (double)GetValue(GridSizeProperty);
            set => SetValue(GridSizeProperty, value);
        }

        public ICommand SnapCommand
        {
            get => (ICommand)GetValue(SnapCommandProperty);
            set => SetValue(SnapCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsSnapEnabled || SnapCommand == null) return;

            var position = e.GetPosition(AssociatedObject);
            var snappedPosition = SnapToGrid(position);

            if (SnapCommand.CanExecute(snappedPosition))
            {
                SnapCommand.Execute(snappedPosition);
            }
        }

        private Point SnapToGrid(Point position)
        {
            if (GridSize <= 0) return position;

            var snappedX = Math.Round(position.X / GridSize) * GridSize;
            var snappedY = Math.Round(position.Y / GridSize) * GridSize;

            return new Point(snappedX, snappedY);
        }
    }
}