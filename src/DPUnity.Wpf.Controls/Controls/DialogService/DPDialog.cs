using HandyControl.Controls;
using System.Threading.Tasks;
using System.Windows;

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


        public static bool? Info(string message, string? title = null)
        {
            return ShowNotification(message, NotificationType.Information, title);
        }

        public static bool? Success(string message, string? title = null)
        {
            return ShowNotification(message, NotificationType.Success, title);
        }

        public static bool? Error(string message, string? title = null)
        {
            return ShowNotification(message, NotificationType.Error, title);
        }

        public static bool? Warning(string message, string? title = null)
        {
            return ShowNotification(message, NotificationType.Warning, title);
        }

        public static bool? Ask(string message, string? title = null)
        {
            return ShowNotification(message, NotificationType.Ask, title);
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

        private static bool? ShowNotification(string message, NotificationType type = NotificationType.Information, string? title = null)
        {
            var window = new Views.NotificationWindow(message, type, title)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
            return window.DialogResult;
        }
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
