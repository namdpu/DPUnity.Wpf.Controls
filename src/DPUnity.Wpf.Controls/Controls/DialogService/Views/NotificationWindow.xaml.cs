using DPUnity.Wpf.UI.Controls.PackIcon;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static DPUnity.Wpf.Controls.Controls.DialogService.DPDialog;

namespace DPUnity.Wpf.Controls.Controls.DialogService.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private NotificationType _type;
        public new bool? DialogResult { get; set; } = null;

        public NotificationWindow(string message, NotificationType type, string? title = null)
        {
            try
            {
                InitializeComponent();

                LoadResourceDictionaries();

                _type = type;
                Message.Text = message;

                switch (type)
                {
                    case NotificationType.Information:
                        NotificationIcon.Kind = PackIconKind.Information;
                        WindowBorder.BorderBrush = FindResource("InfoBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("InfoBrush") as Brush;
                        TitleText.Text = title ?? "Thông tin";
                        TitleText.Foreground = FindResource("InfoBrush") as Brush;
                        break;
                    case NotificationType.Success:
                        NotificationIcon.Kind = PackIconKind.CheckCircle;
                        WindowBorder.BorderBrush = FindResource("SuccessBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("PrimaryBrush") as Brush;
                        TitleText.Text = title ?? "Thành công";
                        TitleText.Foreground = FindResource("PrimaryBrush") as Brush;
                        break;
                    case NotificationType.Error:
                        NotificationIcon.Kind = PackIconKind.CloseCircle;
                        WindowBorder.BorderBrush = FindResource("DangerBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("DangerBrush") as Brush;
                        TitleText.Text = title ?? "Lỗi";
                        TitleText.Foreground = FindResource("DangerBrush") as Brush;
                        break;
                    case NotificationType.Warning:
                        NotificationIcon.Kind = PackIconKind.AlertCircle;
                        WindowBorder.BorderBrush = FindResource("WarningBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("WarningBrush") as Brush;
                        TitleText.Text = title ?? "Cảnh báo";
                        TitleText.Foreground = FindResource("WarningBrush") as Brush;
                        break;
                    case NotificationType.Ask:
                        OKButton.Content = "Yes";
                        NotificationIcon.Kind = PackIconKind.HelpCircle;
                        WindowBorder.BorderBrush = FindResource("AskBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("AskBrush") as Brush;
                        TitleText.Text = title ?? "Xác nhận";
                        TitleText.Foreground = FindResource("AskBrush") as Brush;
                        CancelButton.Visibility = Visibility.Visible;
                        OKButton.Visibility = Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing notification window: {ex.Message}");
            }
        }

        private static ResourceDictionary DPUDict { get; } = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/DPUnity.WPF.UI;component/Styles/DPUnityResources.xaml")
        };
        private static ResourceDictionary HandyDict { get; } = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/DPUnity.WPF.UI;component/Styles/HandyResources.xaml")
        };

        private void LoadResourceDictionaries()
        {
            try
            {
                this.Resources.MergedDictionaries.Clear();

                this.Resources.MergedDictionaries.Add(HandyDict);
                this.Resources.MergedDictionaries.Add(DPUDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Warning: Could not load resource dictionaries: {ex.Message}");
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_type == NotificationType.Ask)
            {
                this.DialogResult = null;
            }
            else
            {
                this.DialogResult = true;
            }
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!this.DialogResult.HasValue)
            {
                if (_type == NotificationType.Ask)
                {
                    this.DialogResult = null;
                }
                else
                {
                    this.DialogResult = true;
                }
            }
            base.OnClosing(e);
        }
    }
}