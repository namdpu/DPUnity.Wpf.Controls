using System.Windows;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.Behaviors
{
    public static class ValidationBehavior
    {
        public static readonly DependencyProperty ValidateOnSubmitProperty =
            DependencyProperty.RegisterAttached("ValidateOnSubmit", typeof(bool), typeof(ValidationBehavior),
                new PropertyMetadata(false, OnValidateOnSubmitChanged));

        public static bool GetValidateOnSubmit(DependencyObject obj)
        {
            return (bool)obj.GetValue(ValidateOnSubmitProperty);
        }

        public static void SetValidateOnSubmit(DependencyObject obj, bool value)
        {
            obj.SetValue(ValidateOnSubmitProperty, value);
        }

        private static void OnValidateOnSubmitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && (bool)e.NewValue)
            {
                var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                if (bindingExpression != null)
                {
                    bindingExpression.UpdateSource();
                }
            }
        }
    }
}
