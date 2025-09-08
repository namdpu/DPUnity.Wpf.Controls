using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService.Views;
using DPUnity.Wpf.Controls.Interfaces;
using HandyControl.Controls;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace DPUnity.Wpf.Controls.Controls.DialogService
{
    public static class DPDialog
    {
        #region Weak Notification
        /// <summary>
        /// Shows a weak notification with the specified message and type.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <returns> true </returns>
        public static async Task<bool?> WeakInfo(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Information);
        }

        /// <summary>
        /// Shows a weak notification with the specified message and type.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <returns> true </returns>
        public static async Task<bool?> WeakSuccess(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Success);
        }

        /// <summary>
        /// Shows a weak notification with the specified message and type.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <returns> true </returns>
        public static async Task<bool?> WeakError(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Error);
        }
        /// <summary>
        /// Shows a weak notification with the specified message and type.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <returns> true </returns>
        public static async Task<bool?> WeakWarning(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Warning);
        }

        /// <summary>
        /// Shows a weak notification with the specified message and type.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <returns> True if the user confirmed the action; null if the action was canceled, otherwise, false.</returns>
        public static async Task<bool?> WeakAsk(string message)
        {
            return await ShowWeakNotification(message, NotificationType.Ask);
        }
        #endregion


        #region Show Notification
        /// <summary>
        /// Shows a notification with a infomation message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="owner">The owner window.</param>
        /// <param name="title">The title of the notification window.</param>
        /// <returns>True</returns>
        public static bool? Info(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Information, owner, title);
        }

        /// <summary>
        /// Shows a notification with a success message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="owner">The owner window.</param>
        /// <param name="title">The title of the notification window.</param>
        /// <returns>True</returns>
        public static bool? Success(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Success, owner, title);
        }

        /// <summary>
        /// Shows a notification with a error message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="owner">The owner window.</param>
        /// <param name="title">The title of the notification window.</param>
        /// <returns>True</returns>
        public static bool? Error(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Error, owner, title);
        }


        /// <summary>
        /// Shows a notification with a warning message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="owner">The owner window.</param>
        /// <param name="title">The title of the notification window.</param>
        /// <returns>True</returns>
        public static bool? Warning(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Warning, owner, title);
        }

        /// <summary>
        /// Shows a notification asking the user to confirm an action.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="owner">The owner window.</param>
        /// <param name="title">The title of the notification window.</param>
        /// <returns>True if the user confirmed the action; null if the action was canceled, otherwise, false.</returns>
        public static bool? Ask(string message, System.Windows.Window? owner = null, string? title = null)
        {
            return ShowNotification(message, NotificationType.Ask, owner, title);
        }
        #endregion

        #region Private Methods
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

        //private static bool? ShowNotification(string message, NotificationType type, System.Windows.Window? owner = null, string? title = null)
        //{
        //    Views.NotificationWindow? window = null;
        //    try
        //    {
        //        window = new(message, type, title);

        //        // Hierarchy of owner windows:
        //        // 1. If owner is provided, use it.
        //        // 2. If no owner is provided, find the active window that is visible.
        //        // 3. If no active window is found, use the main application window.
        //        // 4. If no main window, use the application window handle if available.
        //        if (owner is not null && owner.IsLoaded && owner != window)
        //        {
        //            window.Owner = owner;
        //        }
        //        else
        //        {
        //            WindowHelper.SetWindowOwner(window);
        //        }
        //        window.ShowDialog();
        //        return window.DialogResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error showing notification: {ex.Message}");
        //        window?.Close();
        //        return null;
        //    }
        //}

        private static bool? ShowNotification(string message, NotificationType type, System.Windows.Window? owner = null, string? title = null)
        {
            try
            {
                var (width, height) = CalculateWindowSize(message, type);
                var windowOptions = new WindowOptions
                {
                    Width = width,
                    Height = height,
                    MinWidth = width,
                    MinHeight = height,
                    Title = title ?? GetDefaultTitle(type),
                    ResizeMode = ResizeMode.NoResize
                };
                NotificationViewModel? viewModel = null;
                WindowManager.ShowDialog<IDPDialogWindow, NotificationPage>(windowOptions, (vm) =>
                {
                    if (vm is NotificationViewModel nvm)
                    {
                        nvm.Initialize(message, type, title);
                        viewModel = nvm;
                    }
                });

                if (viewModel != null)
                {
                    return viewModel.DialogResult;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Fallback về MessageBox nếu có lỗi
                MessageBox.Show($"Lỗi hiển thị notification: {ex.Message}\n\nMessage: {message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static (double width, double height) CalculateWindowSize(string message, NotificationType type)
        {
            double width;
            double height;
            int newLineCount = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (newLineCount == 0) newLineCount = 1; // Đảm bảo ít nhất 1 dòng
            double lineHeight = 14 * 1.5;
            double desiredHeight = Math.Min(600, 20 + newLineCount * lineHeight); // Chiều cao tối đa 600, tối thiểu 100
            double desiredWidth = Math.Max(desiredHeight * 2, 5 * message.Length / newLineCount);
            while (desiredWidth > 2 * desiredHeight)
            {
                desiredHeight *= 1.05;
                desiredWidth *= 0.95;
            }
            if (type == NotificationType.Ask)
            {
                desiredHeight += 100; // Tăng thêm chiều cao 100 nếu type là Ask do có nút Yes/No
            }
            width = Math.Max(300, desiredWidth);
            height = Math.Max(100, desiredHeight);
            return (width, height);
        }

        /// <summary>
        /// Lấy title mặc định theo type
        /// </summary>
        private static string GetDefaultTitle(NotificationType type)
        {
            return type switch
            {
                NotificationType.Information => "Thông tin",
                NotificationType.Success => "Thành công",
                NotificationType.Error => "Lỗi",
                NotificationType.Warning => "Cảnh báo",
                NotificationType.Ask => "Xác nhận",
                _ => "Thông báo"
            };
        }
        #endregion

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
