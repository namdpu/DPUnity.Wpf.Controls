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
        private string _message = string.Empty;
        public new bool? DialogResult { get; set; } = null;

        private bool isResizing = false;
        private Point startPos;
        private double startWidth;
        private double startHeight;
        private double startLeft;
        private double startTop;
        private ResizeDirection resizeDirection;

        private enum ResizeDirection
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public NotificationWindow(string message, NotificationType type, string? title = null)
        {
            try
            {
                InitializeComponent();

                LoadResourceDictionaries();

                _type = type;
                _message = message;
                Message.Text = message;
                Message.TextWrapping = TextWrapping.Wrap; // Đảm bảo wrap để tính height linh hoạt

                // Add keyboard event handler
                this.KeyDown += NotificationWindow_KeyDown;

                // Add resize event handlers
                this.MouseLeftButtonUp += Window_MouseLeftButtonUp;
                this.MouseMove += Window_MouseMove;

                // Add loaded event for dynamic sizing
                this.Loaded += NotificationWindow_Loaded;

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
                        this.Loaded += (s, e) => OKButton.Focus();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing notification window: {ex.Message}");
            }
        }

        private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int newLineCount = _message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            double lineHeight = Message.FontSize * 1.5; // Ước tính chiều cao mỗi dòng

            double desiredHeight = Math.Min(600, 20 + newLineCount * lineHeight); // Chiều cao tối đa 600, tối thiểu 100
            double desiredWidth = Math.Max(desiredHeight * 1.4, 5 * _message.Length / newLineCount);
            while (desiredWidth > 1.6 * desiredHeight)
            {
                desiredHeight *= 1.05;
                desiredWidth *= 0.95;
            }
            this.Width = Math.Max(300, desiredWidth);
            this.Height = Math.Max(100, desiredHeight);
            this.UpdateLayout(); // Áp dụng cuối, nếu desired height > max, scroll sẽ xuất hiện
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

        private void Resize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isResizing = true;
                startPos = e.GetPosition(this);
                startWidth = this.Width;
                startHeight = this.Height;
                startLeft = this.Left;
                startTop = this.Top;

                // Xác định hướng resize dựa trên tên control
                if (sender is FrameworkElement element)
                {
                    resizeDirection = element.Name switch
                    {
                        "LeftResize" => ResizeDirection.Left,
                        "RightResize" => ResizeDirection.Right,
                        "TopResize" => ResizeDirection.Top,
                        "BottomResize" => ResizeDirection.Bottom,
                        "TopLeftResize" => ResizeDirection.TopLeft,
                        "TopRightResize" => ResizeDirection.TopRight,
                        "BottomLeftResize" => ResizeDirection.BottomLeft,
                        "BottomRightResize" => ResizeDirection.BottomRight,
                        _ => ResizeDirection.None,
                    };
                }

                this.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                Point currentPos = e.GetPosition(this);
                double deltaX = currentPos.X - startPos.X;
                double deltaY = currentPos.Y - startPos.Y;

                switch (resizeDirection)
                {
                    case ResizeDirection.Left:
                        this.Left += deltaX;
                        this.Width = Math.Max(this.MinWidth, Math.Min(this.MaxWidth, startWidth - deltaX));
                        break;
                    case ResizeDirection.Right:
                        this.Width = Math.Max(this.MinWidth, Math.Min(this.MaxWidth, startWidth + deltaX));
                        break;
                    case ResizeDirection.Top:
                        this.Top += deltaY;
                        this.Height = Math.Max(this.MinHeight, Math.Min(this.MaxHeight, startHeight - deltaY));
                        break;
                    case ResizeDirection.Bottom:
                        this.Height = Math.Max(this.MinHeight, Math.Min(this.MaxHeight, startHeight + deltaY));
                        break;
                    case ResizeDirection.TopLeft:
                        this.Left += deltaX;
                        this.Width = Math.Max(this.MinWidth, Math.Min(this.MaxWidth, startWidth - deltaX));
                        this.Top += deltaY;
                        this.Height = Math.Max(this.MinHeight, Math.Min(this.MaxHeight, startHeight - deltaY));
                        break;
                    case ResizeDirection.TopRight:
                        this.Width = Math.Max(this.MinWidth, Math.Min(this.MaxWidth, startWidth + deltaX));
                        this.Top += deltaY;
                        this.Height = Math.Max(this.MinHeight, Math.Min(this.MaxHeight, startHeight - deltaY));
                        break;
                    case ResizeDirection.BottomLeft:
                        this.Left += deltaX;
                        this.Width = Math.Max(this.MinWidth, Math.Min(this.MaxWidth, startWidth - deltaX));
                        this.Height = Math.Max(this.MinHeight, Math.Min(this.MaxHeight, startHeight + deltaY));
                        break;
                    case ResizeDirection.BottomRight:
                        this.Width = Math.Max(this.MinWidth, Math.Min(this.MaxWidth, startWidth + deltaX));
                        this.Height = Math.Max(this.MinHeight, Math.Min(this.MaxHeight, startHeight + deltaY));
                        break;
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                isResizing = false;
                this.ReleaseMouseCapture();
                resizeDirection = ResizeDirection.None;
            }
        }

        private void NotificationWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (_type == NotificationType.Ask)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        this.DialogResult = null;
                        this.Close();
                        e.Handled = true;
                        break;
                    case Key.Enter:
                    case Key.Space:
                        if (OKButton.IsFocused)
                        {
                            OKButton_Click(OKButton, new RoutedEventArgs());
                        }
                        else if (CancelButton.IsFocused)
                        {
                            CancelButton_Click(CancelButton, new RoutedEventArgs());
                        }
                        else
                        {
                            OKButton_Click(OKButton, new RoutedEventArgs());
                        }
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Escape:
                    case Key.Enter:
                    case Key.Space:
                        CloseButton_Click(CloseButton, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                }
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