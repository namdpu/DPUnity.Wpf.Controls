//using DPUnity.Windows;
//using DPUnity.Wpf.Common.Windows;
//using DPUnity.Wpf.Controls.Controls.DialogService.Views;
//using DPUnity.Wpf.Controls.Interfaces;
//using HandyControl.Controls;
//using System.Windows;
//using MessageBox = System.Windows.MessageBox;

//namespace DPUnity.Wpf.Controls.Controls.DialogService
//{
//    public enum NotificationType
//    {
//        Information,
//        Success,
//        Error,
//        Warning,
//        Ask
//    }
//    public class DPDialog : IDPDialog
//    {
//        private const double ASK_HEIGHT_INCREASE = 100;
//        private const double MIN_WIDTH = 300;
//        private const double MIN_HEIGHT = 100;
//        private const double MAX_HEIGHT = 600;
//        private const double LINE_HEIGHT = 14 * 1.5;
//        private const double WIDTH_HEIGHT_RATIO_LIMIT = 2.0;
//        private const double WIDTH_DECREASE_FACTOR = 0.95;
//        private const double HEIGHT_INCREASE_FACTOR = 1.05;
//        private const double BASE_HEIGHT_PADDING = 20;
//        private const double MESSAGE_LENGTH_FACTOR = 5;
//        private readonly IWindowManager _windowManager;

//        public DPDialog(IWindowManager windowManager)
//        {
//            _windowManager = windowManager;
//        }



//        #region Weak Notification
//        /// <summary>
//        /// Shows a weak notification with the specified message and type.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <returns> true </returns>
//        public async Task<bool?> WeakInfo(string message)
//        {
//            return await ShowWeakNotification(message, NotificationType.Information);
//        }

//        /// <summary>
//        /// Shows a weak notification with the specified message and type.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <returns> true </returns>
//        public async Task<bool?> WeakSuccess(string message)
//        {
//            return await ShowWeakNotification(message, NotificationType.Success);
//        }

//        /// <summary>
//        /// Shows a weak notification with the specified message and type.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <returns> true </returns>
//        public async Task<bool?> WeakError(string message)
//        {
//            return await ShowWeakNotification(message, NotificationType.Error);
//        }
//        /// <summary>
//        /// Shows a weak notification with the specified message and type.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <returns> true </returns>
//        public async Task<bool?> WeakWarning(string message)
//        {
//            return await ShowWeakNotification(message, NotificationType.Warning);
//        }

//        /// <summary>
//        /// Shows a weak notification with the specified message and type.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <returns> True if the user confirmed the action; null if the action was canceled, otherwise, false.</returns>
//        public async Task<bool?> WeakAsk(string message)
//        {
//            return await ShowWeakNotification(message, NotificationType.Ask);
//        }
//        #endregion


//        #region Show Notification
//        /// <summary>
//        /// Shows a notification with a infomation message.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <param name="owner">The owner window.</param>
//        /// <param name="title">The title of the notification window.</param>
//        /// <returns>True</returns>
//        public bool? Info(string message, System.Windows.Window? owner = null, string? title = null)
//        {
//            return ShowNotification(message, NotificationType.Information, owner, title);
//        }

//        /// <summary>
//        /// Shows a notification with a success message.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <param name="owner">The owner window.</param>
//        /// <param name="title">The title of the notification window.</param>
//        /// <returns>True</returns>
//        public bool? Success(string message, System.Windows.Window? owner = null, string? title = null)
//        {
//            return ShowNotification(message, NotificationType.Success, owner, title);
//        }

//        /// <summary>
//        /// Shows a notification with a error message.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <param name="owner">The owner window.</param>
//        /// <param name="title">The title of the notification window.</param>
//        /// <returns>True</returns>
//        public bool? Error(string message, System.Windows.Window? owner = null, string? title = null)
//        {
//            return ShowNotification(message, NotificationType.Error, owner, title);
//        }


//        /// <summary>
//        /// Shows a notification with a warning message.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <param name="owner">The owner window.</param>
//        /// <param name="title">The title of the notification window.</param>
//        /// <returns>True</returns>
//        public bool? Warning(string message, System.Windows.Window? owner = null, string? title = null)
//        {
//            return ShowNotification(message, NotificationType.Warning, owner, title);
//        }

//        /// <summary>
//        /// Shows a notification asking the user to confirm an action.
//        /// </summary>
//        /// <param name="message">The message to display.</param>
//        /// <param name="owner">The owner window.</param>
//        /// <param name="title">The title of the notification window.</param>
//        /// <returns>True if the user confirmed the action; null if the action was canceled, otherwise, false.</returns>
//        public bool? Ask(string message, System.Windows.Window? owner = null, string? title = null)
//        {
//            return ShowNotification(message, NotificationType.Ask, owner, title);
//        }
//        #endregion

//        #region Private Methods
//        private async Task<bool?> ShowWeakNotification(string message, NotificationType type = NotificationType.Information)
//        {
//            switch (type)
//            {
//                case NotificationType.Success:
//                    Growl.Success(message);
//                    return true;
//                case NotificationType.Error:
//                    Growl.Error(message);
//                    return true;
//                case NotificationType.Warning:
//                    Growl.Warning(message);
//                    return true;
//                case NotificationType.Information:
//                    Growl.Info(message);
//                    return true;
//                case NotificationType.Ask:
//                    var tcs = new TaskCompletionSource<bool>();
//                    Growl.Ask(message, isConfirmed =>
//                    {
//                        tcs.SetResult(isConfirmed);
//                        return true;
//                    });
//                    return await tcs.Task;
//                default:
//                    Growl.Info(message);
//                    return true;
//            }
//        }

//        private bool? ShowNotification(string message, NotificationType type, System.Windows.Window? owner = null, string? title = null)
//        {
//            try
//            {
//                var (width, height) = CalculateWindowSize(message, type);
//                var windowOptions = new WindowOptions
//                {
//                    Width = width,
//                    Height = height,
//                    MinWidth = width,
//                    MinHeight = height,
//                    Title = title ?? GetDefaultTitle(type),
//                    ResizeMode = ResizeMode.NoResize
//                };
//                NotificationViewModel? viewModel = null;
//                _windowManager.ShowDialog<IDPDialogWindow, NotificationPage>(windowOptions, (vm) =>
//                {
//                    if (vm is NotificationViewModel nvm)
//                    {
//                        nvm.Initialize(message, type, title);
//                        viewModel = nvm;
//                    }
//                });

//                if (viewModel != null)
//                {
//                    return viewModel.DialogResult;
//                }
//                return false;
//            }
//            catch (Exception ex)
//            {
//                // Fallback về MessageBox nếu có lỗi
//                MessageBox.Show($"Lỗi hiển thị notification: {ex.Message}\n\nMessage: {message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//                return null;
//            }
//        }

//        private static (double width, double height) CalculateWindowSize(string message, NotificationType type)
//        {
//            double width;
//            double height;
//            int newLineCount = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
//            if (newLineCount == 0) newLineCount = 1; // Đảm bảo ít nhất 1 dòng
//            double desiredHeight = Math.Min(MAX_HEIGHT, BASE_HEIGHT_PADDING + newLineCount * LINE_HEIGHT);
//            double desiredWidth = Math.Max(desiredHeight * WIDTH_HEIGHT_RATIO_LIMIT, MESSAGE_LENGTH_FACTOR * message.Length / newLineCount);
//            while (desiredWidth > WIDTH_HEIGHT_RATIO_LIMIT * desiredHeight)
//            {
//                desiredHeight *= HEIGHT_INCREASE_FACTOR;
//                desiredWidth *= WIDTH_DECREASE_FACTOR;
//            }
//            if (type == NotificationType.Ask)
//            {
//                desiredHeight += ASK_HEIGHT_INCREASE; // Tăng thêm chiều cao ASK_HEIGHT_INCREASE nếu type là Ask do có nút Yes/No
//            }
//            width = Math.Max(MIN_WIDTH, desiredWidth);
//            height = Math.Max(MIN_HEIGHT, desiredHeight);
//            return (width, height);
//        }

//        /// <summary>
//        /// Lấy title mặc định theo type
//        /// </summary>
//        private static string GetDefaultTitle(NotificationType type)
//        {
//            return type switch
//            {
//                NotificationType.Information => "Thông tin",
//                NotificationType.Success => "Thành công",
//                NotificationType.Error => "Lỗi",
//                NotificationType.Warning => "Cảnh báo",
//                NotificationType.Ask => "Xác nhận",
//                _ => "Thông báo"
//            };
//        }
//        #endregion

       
//    }
//}
