using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Wpf.Common.Controls;
using DPUnity.Wpf.Common.Windows;
using DPUnity.Wpf.Controls.Helpers;
using DPUnity.Wpf.UI.Controls.PackIcon;
using System.Windows;
using System.Windows.Media;

namespace DPUnity.Wpf.Controls.Controls.DialogService.Views
{
    public partial class NotificationViewModel : ViewModelPage
    {
        public bool? DialogResult { get; private set; } = null;

        [ObservableProperty]
        private NotificationType type = NotificationType.Information;

        [ObservableProperty]
        private string message = string.Empty;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private PackIconKind iconKind = PackIconKind.Information;

        [ObservableProperty]
        private Brush? borderBrush;

        [ObservableProperty]
        private Brush? iconForeground;

        [ObservableProperty]
        private Brush? titleForeground;

        [ObservableProperty]
        private Visibility okButtonVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility cancelButtonVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string okButtonContent = "OK";

        [ObservableProperty]
        private string cancelButtonContent = "Cancel";

        public NotificationViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        /// <summary>
        /// Khởi tạo ViewModel với thông tin thông báo
        /// </summary>
        /// <param name="message">Nội dung thông báo</param>
        /// <param name="type">Loại thông báo</param>
        /// <param name="title">Tiêu đề thông báo (nếu null sẽ sử dụng tiêu đề mặc định theo loại)</param>
        public void Initialize(string message, NotificationType type, string? title = null)
        {
            Message = message;
            Type = type;
            ConfigureByType(title);
            WindowHelper.SetWindowOwner(WindowService.CurrentWindow.Window);
        }

        /// <summary>
        /// Cấu hình ViewModel theo loại thông báo
        /// </summary>
        /// <param name="customTitle">Tiêu đề tùy chỉnh, nếu null sẽ sử dụng tiêu đề mặc định</param>
        public void ConfigureByType(string? customTitle = null)
        {
            // Lấy resources từ Application hoặc từ merged dictionaries
            var application = Application.Current;

            switch (Type)
            {
                case NotificationType.Information:
                    IconKind = PackIconKind.Information;
                    BorderBrush = GetBrushResource("InfoBrush", application);
                    IconForeground = GetBrushResource("InfoBrush", application);
                    Title = customTitle ?? "Thông tin";
                    TitleForeground = GetBrushResource("InfoBrush", application);
                    SetupInformationButtons();
                    break;

                case NotificationType.Success:
                    IconKind = PackIconKind.CheckCircle;
                    BorderBrush = GetBrushResource("SuccessBrush", application);
                    IconForeground = GetBrushResource("PrimaryBrush", application);
                    Title = customTitle ?? "Thành công";
                    TitleForeground = GetBrushResource("PrimaryBrush", application);
                    SetupInformationButtons();
                    break;

                case NotificationType.Error:
                    IconKind = PackIconKind.CloseCircle;
                    BorderBrush = GetBrushResource("DangerBrush", application);
                    IconForeground = GetBrushResource("DangerBrush", application);
                    Title = customTitle ?? "Lỗi";
                    TitleForeground = GetBrushResource("DangerBrush", application);
                    SetupInformationButtons();
                    break;

                case NotificationType.Warning:
                    IconKind = PackIconKind.AlertCircle;
                    BorderBrush = GetBrushResource("WarningBrush", application);
                    IconForeground = GetBrushResource("WarningBrush", application);
                    Title = customTitle ?? "Cảnh báo";
                    TitleForeground = GetBrushResource("WarningBrush", application);
                    SetupInformationButtons();
                    break;
                case NotificationType.Ask:
                    IconKind = PackIconKind.HelpCircle;
                    BorderBrush = GetBrushResource("AskBrush", application);
                    IconForeground = GetBrushResource("AskBrush", application);
                    Title = customTitle ?? "Xác nhận";
                    TitleForeground = GetBrushResource("AskBrush", application);
                    SetupConfirmationButtons();
                    break;
            }
        }

        /// <summary>
        /// Thiết lập các nút cho loại thông báo thông tin (chỉ có nút OK/Close)
        /// </summary>
        private void SetupInformationButtons()
        {
            OkButtonVisibility = Visibility.Collapsed;
            CancelButtonVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Thiết lập các nút cho loại thông báo xác nhận (có cả nút Yes/No)
        /// </summary>
        private void SetupConfirmationButtons()
        {
            OkButtonVisibility = Visibility.Visible;
            CancelButtonVisibility = Visibility.Visible;
            OkButtonContent = "Yes";
            CancelButtonContent = "No";
        }

        /// <summary>
        /// Lấy Brush resource từ Application resources
        /// </summary>
        private static Brush? GetBrushResource(string resourceKey, Application? application)
        {
            try
            {
                if (application?.TryFindResource(resourceKey) is Brush brush)
                {
                    return brush;
                }

                // Fallback colors nếu không tìm thấy resource
                return resourceKey switch
                {
                    "InfoBrush" => new SolidColorBrush(Colors.DodgerBlue),
                    "SuccessBrush" => new SolidColorBrush(Colors.Green),
                    "PrimaryBrush" => new SolidColorBrush(Colors.Green),
                    "DangerBrush" => new SolidColorBrush(Colors.Red),
                    "WarningBrush" => new SolidColorBrush(Colors.Orange),
                    "AskBrush" => new SolidColorBrush(Colors.RoyalBlue),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            catch
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        [RelayCommand]
        private void OkAction()
        {
            DialogResult = true;
            OK();
            WindowService.CurrentWindow.CloseWindow();
        }

        [RelayCommand]
        private new void Cancel()
        {
            DialogResult = false;
            WindowService.CurrentWindow.CloseWindow();
        }

        [RelayCommand]
        private void Close()
        {
            DialogResult = Type == NotificationType.Ask ? null : true;
            if (Type == NotificationType.Ask && DialogResult == null)
            {
                WindowService.CurrentWindow.CloseWindow();
            }
            else
            {
                WindowService.CurrentWindow.CloseWindow();
            }
        }
    }
}