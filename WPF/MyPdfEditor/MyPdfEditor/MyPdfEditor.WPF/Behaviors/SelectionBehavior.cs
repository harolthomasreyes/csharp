// MyPdfEditor.WPF/Behaviors/SelectionBehavior.cs
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class SelectionBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register(nameof(IsSelectable), typeof(bool), typeof(SelectionBehavior), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register(nameof(SelectCommand), typeof(ICommand), typeof(SelectionBehavior));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SelectionBehavior),
                new PropertyMetadata(false, OnIsSelectedChanged));

        private bool _isMouseDown;

        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        public ICommand SelectCommand
        {
            get => (ICommand)GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsSelectable || e.ChangedButton != MouseButton.Left)
                return;

            _isMouseDown = true;
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsSelectable || !_isMouseDown)
                return;

            if (SelectCommand?.CanExecute(AssociatedObject.DataContext) == true)
            {
                SelectCommand.Execute(AssociatedObject.DataContext);
            }

            _isMouseDown = false;
            e.Handled = true;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsSelectable)
            {
                AssociatedObject.Cursor = Cursors.Hand;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            AssociatedObject.Cursor = Cursors.Arrow;
            _isMouseDown = false;
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (SelectionBehavior)d;
            behavior.UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            // Update visual appearance based on selection state
            if (IsSelected)
            {
                AssociatedObject.Opacity = 1.0;
                // Add selection visual effects
            }
            else
            {
                AssociatedObject.Opacity = 1.0;
                // Remove selection visual effects
            }
        }
    }
}