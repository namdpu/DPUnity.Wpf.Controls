using DPUnity.Windows.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DPUnity.Wpf.Controls.Helpers
{
    public class ValidationHelper
    {
        /// <summary>
        /// Checks if there are any validation errors in the visual tree of the main window in the given window service.
        /// </summary>
        /// <param name="windowService">The window service which main window is needed to check for validation errors.</param>
        /// <returns></returns>
        public static bool HasValidationFail(IWindowService windowService)
        {
            bool result = false;
            var window = windowService.CurrentWindow.Window;
            if (window.FindName("MainFrame") is Frame frame && frame.Content is Page page)
            {
                foreach (var element in page.FindVisualChildren<FrameworkElement>())
                {
                    if (element is TextBox textBox)
                    {
                        var binding = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
                        binding?.UpdateSource();
                        if (Validation.GetHasError(textBox))
                            result = true;
                    }
                    else if (element is ComboBox comboBox)
                    {
                        var binding = BindingOperations.GetBindingExpression(comboBox, ComboBox.SelectedItemProperty);
                        binding?.UpdateSource();
                        if (Validation.GetHasError(comboBox))
                            result = true;

                    }
                }
            }
            return result;
        }
    }
}
