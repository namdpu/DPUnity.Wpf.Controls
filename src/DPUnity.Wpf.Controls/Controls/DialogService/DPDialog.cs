using HandyControl.Controls;
using System.Windows;
using System.Windows.Interop;
using MessageBox = System.Windows.MessageBox;

namespace DPUnity.Wpf.Controls.Controls.DialogService
{
    public static class DPDialog
    {
        public static async Task<bool?> WeakInfo(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Information);
        }

        public static async Task<bool?> WeakSuccess(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Success);
        }

        public static async Task<bool?> WeakError(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Error);
        }

        public static async Task<bool?> WeakWarning(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Warning);
        }

        public static async Task<bool?> WeakAsk(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Ask);
        }


        public static bool? Info(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Information, owner, title);
        }

        public static bool? Success(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Success, owner, title);
        }

        public static bool? Error(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Error, owner, title);
        }

        public static bool? Warning(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Warning, owner, title);
        }

        public static bool? Ask(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Ask, owner, title);
        }

        public static bool? Info(string message, IntPtr intPtr, string? title = null)
        {
            return ShowNotification(message, NotificationType.Information, intPtr, title);
        }

        public static bool? Success(string message, IntPtr intPtr, string? title = null)
        {
            return ShowNotification(message, NotificationType.Success, intPtr, title);
        }

        public static bool? Error(string message, IntPtr intPtr, string? title = null)
        {
            return ShowNotification(message, NotificationType.Error, intPtr, title);
        }

        public static bool? Warning(string message, IntPtr intPtr, string? title = null)
        {
            return ShowNotification(message, NotificationType.Warning, intPtr, title);
        }

        public static bool? Ask(string message, IntPtr intPtr, string? title = null)
        {
            return ShowNotification(message, NotificationType.Ask, intPtr, title);
        }

        private static async Task<bool?> ShowWeakNotification(string message, NotificationType type = NotificationType.Information)
        {
            switch (type)
            {
                case NotificationType.Success:
                    Growl.Success(message);
                    return true;
                case NotificationType.Error:
                    Growl.Error(message);
                    return true;
                case NotificationType.Warning:
                    Growl.Warning(message);
                    return true;
                case NotificationType.Information:
                    Growl.Info(message);
                    return true;
                case NotificationType.Ask:
                    var tcs = new TaskCompletionSource<bool>();
                    Growl.Ask(message, isConfirmed =>
                    {
                        tcs.SetResult(isConfirmed);
                        return true;
                    });
                    return await tcs.Task;
                default:
                    Growl.Info(message);
                    return true;
            }
        }

        private static bool? ShowNotification(string message, NotificationType type = NotificationType.Information, System.Windows.Window? owner = null, string? title = null)
        {
            Views.NotificationWindow? window = null;
            try
            {
                WindowStartupLocation location = WindowStartupLocation.CenterOwner;
                var wd = owner ?? Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow;

                if (wd == null || window == wd)
                {
                    location = WindowStartupLocation.CenterScreen;
                }

                window = new(message, type, title);
                window.WindowStartupLocation = location;
                if (wd != null && window != wd)
                {
                    window.Owner = wd;

                }
                window.ShowDialog();
                return window.DialogResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing notification: {ex.Message}");
                window?.Close();
                return null;
            }
        }

        private static bool? ShowNotification(string message, NotificationType type, IntPtr intPtr, string? title = null)
        {
            var window = new Views.NotificationWindow(message, type, title);

            var helper = new WindowInteropHelper(window)
            {
                Owner = intPtr
            };
            window.ShowDialog();
            return window.DialogResult;
        }

        public enum NotificationType
        {
            Information,
            Success,
            Error,
            Warning,
            Ask
        }
    }
}
