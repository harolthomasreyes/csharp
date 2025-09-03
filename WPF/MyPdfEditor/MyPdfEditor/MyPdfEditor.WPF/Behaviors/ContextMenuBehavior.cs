// MyPdfEditor.WPF/Behaviors/ContextMenuBehavior.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;

namespace MyPdfEditor.WPF.Behaviors
{
    public class ContextMenuBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ContextMenuCommandProperty =
            DependencyProperty.Register(nameof(ContextMenuCommand), typeof(ICommand), typeof(ContextMenuBehavior));

        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register(nameof(MenuItems), typeof(ContextMenu), typeof(ContextMenuBehavior));

        private ContextMenu _contextMenu;

        public ICommand ContextMenuCommand
        {
            get => (ICommand)GetValue(ContextMenuCommandProperty);
            set => SetValue(ContextMenuCommandProperty, value);
        }

        public ContextMenu MenuItems
        {
            get => (ContextMenu)GetValue(MenuItemsProperty);
            set => SetValue(MenuItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseRightButtonDown += OnMouseRightButtonDown;
            AssociatedObject.ContextMenuOpening += OnContextMenuOpening;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseRightButtonDown -= OnMouseRightButtonDown;
            AssociatedObject.ContextMenuOpening -= OnContextMenuOpening;
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ContextMenuCommand?.CanExecute(AssociatedObject.DataContext) == true)
            {
                ContextMenuCommand.Execute(AssociatedObject.DataContext);
                e.Handled = true;
            }
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (MenuItems != null)
            {
                AssociatedObject.ContextMenu = MenuItems;
            }
        }
    }
}