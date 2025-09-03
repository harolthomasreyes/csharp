// MyPdfEditor.WPF/Behaviors/KeyboardNavigationBehavior.cs
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class KeyboardNavigationBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register(nameof(MoveCommand), typeof(ICommand), typeof(KeyboardNavigationBehavior));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(KeyboardNavigationBehavior));

        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register(nameof(CopyCommand), typeof(ICommand), typeof(KeyboardNavigationBehavior));

        public static readonly DependencyProperty PasteCommandProperty =
            DependencyProperty.Register(nameof(PasteCommand), typeof(ICommand), typeof(KeyboardNavigationBehavior));

        public ICommand MoveCommand
        {
            get => (ICommand)GetValue(MoveCommandProperty);
            set => SetValue(MoveCommandProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public ICommand CopyCommand
        {
            get => (ICommand)GetValue(CopyCommandProperty);
            set => SetValue(CopyCommandProperty, value);
        }

        public ICommand PasteCommand
        {
            get => (ICommand)GetValue(PasteCommandProperty);
            set => SetValue(PasteCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.Focusable = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                HandleKeyPress(e);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                HandleControlKeyPress(e);
            }
        }

        private void HandleKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ExecuteMoveCommand(0, -1);
                    e.Handled = true;
                    break;
                case Key.Down:
                    ExecuteMoveCommand(0, 1);
                    e.Handled = true;
                    break;
                case Key.Left:
                    ExecuteMoveCommand(-1, 0);
                    e.Handled = true;
                    break;
                case Key.Right:
                    ExecuteMoveCommand(1, 0);
                    e.Handled = true;
                    break;
                case Key.Delete:
                    ExecuteCommand(DeleteCommand);
                    e.Handled = true;
                    break;
            }
        }

        private void HandleControlKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.C:
                    ExecuteCommand(CopyCommand);
                    e.Handled = true;
                    break;
                case Key.V:
                    ExecuteCommand(PasteCommand);
                    e.Handled = true;
                    break;
                case Key.X:
                    ExecuteCommand(DeleteCommand);
                    e.Handled = true;
                    break;
                case Key.D:
                    ExecuteMoveCommand(1, 1); // Duplicate and move slightly
                    e.Handled = true;
                    break;
            }
        }

        private void ExecuteMoveCommand(double deltaX, double deltaY)
        {
            if (MoveCommand?.CanExecute(new MoveParameters(deltaX, deltaY)) == true)
            {
                MoveCommand.Execute(new MoveParameters(deltaX, deltaY));
            }
        }

        private void ExecuteCommand(ICommand command)
        {
            if (command?.CanExecute(AssociatedObject.DataContext) == true)
            {
                command.Execute(AssociatedObject.DataContext);
            }
        }
    }

    public class MoveParameters
    {
        public double DeltaX { get; }
        public double DeltaY { get; }

        public MoveParameters(double deltaX, double deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }
}