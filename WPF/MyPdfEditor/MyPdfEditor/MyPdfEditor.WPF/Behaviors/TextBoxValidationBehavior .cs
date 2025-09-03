// MyPdfEditor.WPF/Behaviors/TextBoxValidationBehavior.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;

namespace MyPdfEditor.WPF.Behaviors
{
    public class TextBoxValidationBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty ValidationPatternProperty =
            DependencyProperty.Register(nameof(ValidationPattern), typeof(string), typeof(TextBoxValidationBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(nameof(IsRequired), typeof(bool), typeof(TextBoxValidationBehavior),
                new PropertyMetadata(false));

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(TextBoxValidationBehavior),
                new PropertyMetadata(0));

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(TextBoxValidationBehavior),
                new PropertyMetadata(true));

        public string ValidationPattern
        {
            get => (string)GetValue(ValidationPatternProperty);
            set => SetValue(ValidationPatternProperty, value);
        }

        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
            AssociatedObject.LostFocus += OnLostFocus;
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= OnTextChanged;
            AssociatedObject.LostFocus -= OnLostFocus;
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateText();
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ValidateText();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (MaxLength > 0 && AssociatedObject.Text.Length + e.Text.Length > MaxLength)
            {
                e.Handled = true;
                return;
            }

            if (!string.IsNullOrEmpty(ValidationPattern))
            {
                var newText = AssociatedObject.Text.Insert(AssociatedObject.CaretIndex, e.Text);
                if (!Regex.IsMatch(newText, ValidationPattern))
                {
                    e.Handled = true;
                }
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.SourceDataObject.GetData(DataFormats.Text);

                if (MaxLength > 0 && AssociatedObject.Text.Length + text.Length > MaxLength)
                {
                    e.CancelCommand();
                    return;
                }

                if (!string.IsNullOrEmpty(ValidationPattern))
                {
                    var newText = AssociatedObject.Text.Insert(AssociatedObject.CaretIndex, text);
                    if (!Regex.IsMatch(newText, ValidationPattern))
                    {
                        e.CancelCommand();
                    }
                }
            }
        }

        private void ValidateText()
        {
            var text = AssociatedObject.Text;

            // Check required field
            if (IsRequired && string.IsNullOrWhiteSpace(text))
            {
                IsValid = false;
                return;
            }

            // Check max length
            if (MaxLength > 0 && text.Length > MaxLength)
            {
                IsValid = false;
                return;
            }

            // Check pattern validation
            if (!string.IsNullOrEmpty(ValidationPattern) && !string.IsNullOrEmpty(text))
            {
                IsValid = Regex.IsMatch(text, ValidationPattern);
                return;
            }

            IsValid = true;
        }
    }
}