using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Controls.Controls.DialogService.Views;
using DPUnity.Wpf.Controls.Interfaces;
using System.Windows;
using static DPUnity.Wpf.Controls.Controls.DialogService.DPDialog;

namespace DPUnity.Wpf.Controls.Controls.DialogService
{
    public interface IDPDialogService
    {
        Task<bool?> ShowInfo(string message, string? title = null);
        Task<bool?> ShowSuccess(string message, string? title = null);
        Task<bool?> ShowError(string message, string? title = null);
        Task<bool?> ShowWarning(string message, string? title = null);
        Task<bool?> ShowAsk(string message, string? title = null);
    }

    public class DPDialogService : IDPDialogService
    {
        private readonly IWindowService _windowService;

        public DPDialogService(IWindowService windowService)
        {
            _windowService = windowService;
        }

        /// <summary>
        /// Hiển thị notification với thông tin
        /// </summary>
        public async Task<bool?> ShowInfo(string message, string? title = null)
        {
            return await ShowNotification(message, NotificationType.Information, title);
        }

        /// <summary>
        /// Hiển thị notification thành công
        /// </summary>
        public async Task<bool?> ShowSuccess(string message, string? title = null)
        {
            return await ShowNotification(message, NotificationType.Success, title);
        }

        /// <summary>
        /// Hiển thị notification lỗi
        /// </summary>
        public async Task<bool?> ShowError(string message, string? title = null)
        {
            return await ShowNotification(message, NotificationType.Error, title);
        }

        /// <summary>
        /// Hiển thị notification cảnh báo
        /// </summary>
        public async Task<bool?> ShowWarning(string message, string? title = null)
        {
            return await ShowNotification(message, NotificationType.Warning, title);
        }

        /// <summary>
        /// Hiển thị notification xác nhận (Yes/No)
        /// </summary>
        public async Task<bool?> ShowAsk(string message, string? title = null)
        {
            return await ShowNotification(message, NotificationType.Ask, title);
        }


        #region Private Methods
        /// <summary>
        /// Hiển thị notification với type tùy chỉnh
        /// </summary>
        private async Task<bool?> ShowNotification(string message, NotificationType type, string? title = null)
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
                var (result, viewModel) = await _windowService.OpenWindowDialogByLoadingAsync<IDPDialogWindow, NotificationPage, NotificationViewModel>(
                    windowOptions,
                    false,
                    (vm) =>
                    {
                        if (vm is NotificationViewModel notificationVM)
                        {
                            notificationVM.Initialize(message, type, title);
                        }
                    });

                // Trả về kết quả
                if (viewModel != null)
                {
                    return viewModel.DialogResult;
                }

                return result == MessageResult.OK ? true : null;
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
    }
}
