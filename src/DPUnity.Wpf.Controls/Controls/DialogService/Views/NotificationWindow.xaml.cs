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
                        BackgroundOverlay.Fill = FindResource("InfoBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("InfoBrush") as Brush;
                        TitleText.Text = title ?? "Thông tin";
                        break;
                    case NotificationType.Success:
                        NotificationIcon.Kind = PackIconKind.CheckCircle;
                        BackgroundOverlay.Fill = FindResource("SuccessBrush") as Brush;
                        //NotificationIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 94, 232, 72));
                        NotificationIcon.Foreground = FindResource("PrimaryBrush") as Brush;
                        TitleText.Text = title ?? "Thành công";
                        break;
                    case NotificationType.Error:
                        NotificationIcon.Kind = PackIconKind.CloseCircle;
                        BackgroundOverlay.Fill = FindResource("DangerBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("DangerBrush") as Brush;
                        TitleText.Text = title ?? "Lỗi";
                        break;
                    case NotificationType.Warning:
                        NotificationIcon.Kind = PackIconKind.AlertCircle;
                        BackgroundOverlay.Fill = FindResource("WarningBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("WarningBrush") as Brush;
                        TitleText.Text = title ?? "Cảnh báo";
                        break;
                    case NotificationType.Ask:
                        OKButton.Content = "Yes";
                        NotificationIcon.Kind = PackIconKind.HelpCircle;
                        BackgroundOverlay.Fill = FindResource("AskBrush") as Brush;
                        NotificationIcon.Foreground = FindResource("AskBrush") as Brush;
                        TitleText.Text = title ?? "Xác nhận";
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