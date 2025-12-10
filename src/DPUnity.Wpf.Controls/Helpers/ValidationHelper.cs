using DPUnity.Wpf.Common.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DPUnity.Wpf.Controls.Helpers
{
    public static class ValidationHelper
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
                    if (element.IsEnabled == false)
                    {
                        continue;
                    }
                    // Duyệt qua tất cả local values để tìm và cập nhật binding expressions
                    var enumerator = element.GetLocalValueEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var entry = enumerator.Current;
                        if (BindingOperations.IsDataBound(element, entry.Property))
                        {
                            var bindingExpr = BindingOperations.GetBindingExpression(element, entry.Property);
                            bindingExpr?.UpdateSource();
                        }
                    }

                    // Kiểm tra nếu element có lỗi validation
                    if (Validation.GetHasError(element))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}