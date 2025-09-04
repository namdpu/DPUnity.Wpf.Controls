using DPUnity.Wpf.Controls.Interfaces;
using System.Windows;
using System.Windows.Interop;

namespace DPUnity.Wpf.Controls.Helpers
{
    public static class WindowHelper
    {
        public static void SetWindowOwner(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            var wd = Application.Current.Windows
                        .OfType<System.Windows.Window>()
                        .FirstOrDefault(w => w.IsActive && w.Visibility == Visibility.Visible);

            if (wd != null && wd.IsLoaded && wd != window)
            {
                window.Owner = wd;
                // Ensure CenterOwner is used when owner is set
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                // Try to use the application window handle if available
                var hwnd = AppHwnd.Instance.Hwnd;
                if (hwnd is not null)
                {
                    try
                    {
                        var helper = new WindowInteropHelper(window)
                        {
                            Owner = (IntPtr)hwnd
                        };
                        // When using WindowInteropHelper, we need to set CenterOwner
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }
                    catch
                    {
                        // If setting owner via handle fails, fallback to center screen
                        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                }
                else // If hwnd is not set, default to center screen
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
        }
    }
}
