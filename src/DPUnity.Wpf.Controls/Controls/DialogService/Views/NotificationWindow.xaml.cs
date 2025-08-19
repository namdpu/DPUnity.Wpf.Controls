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
                if (!string.IsNullOrEmpty(title))
                {
                    TitleText.Text = title;
                }
                else
                {
                    TitleText.Visibility = Visibility.Collapsed;
                }

                switch (type)
                {
                    case NotificationType.Information:
                        NotificationIcon.Kind = PackIconKind.Information;
                        NotificationIcon.Foreground = FindResource("InfoBrush") as Brush;
                        Message.Foreground = FindResource("InfoBrush") as Brush;
                        break;
                    case NotificationType.Success:
                        NotificationIcon.Kind = PackIconKind.CheckCircle;
                        NotificationIcon.Foreground = FindResource("SuccessBrush") as Brush;
                        Message.Foreground = FindResource("SuccessBrush") as Brush;
                        break;
                    case NotificationType.Error:
                        NotificationIcon.Kind = PackIconKind.CloseCircle;
                        NotificationIcon.Foreground = FindResource("DangerBrush") as Brush;
                        Message.Foreground = FindResource("DangerBrush") as Brush;
                        break;
                    case NotificationType.Warning:
                        NotificationIcon.Kind = PackIconKind.AlertCircle;
                        NotificationIcon.Foreground = FindResource("WarningBrush") as Brush;
                        Message.Foreground = FindResource("WarningBrush") as Brush;
                        break;
                    case NotificationType.Ask:
                        OKButton.Content = "Yes";
                        NotificationIcon.Kind = PackIconKind.HelpCircle;
                        NotificationIcon.Foreground = FindResource("PrimaryTextBrush") as Brush;
                        Message.Foreground = FindResource("PrimaryTextBrush") as Brush;
                        CancelButton.Visibility = Visibility.Visible;
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
            Source = new Uri("pack://application:,,,/DPUnity.WPF.UI;component/Styles/HandyResources.xaml")
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

                this.Resources.MergedDictionaries.Add(DPUDict);
                this.Resources.MergedDictionaries.Add(HandyDict);
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
