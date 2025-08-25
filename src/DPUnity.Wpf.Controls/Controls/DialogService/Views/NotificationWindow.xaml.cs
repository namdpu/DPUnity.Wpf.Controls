using DPUnity.Wpf.UI.Controls.PackIcon;
using System.Runtime.InteropServices;
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

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

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

                        // Focus vào button đầu tiên và thiết lập tab navigation
                        this.Loaded += (s, e) =>
                        {
                            OKButton.Focus();
                            // Đảm bảo keyboard navigation được kích hoạt
                            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
                        };
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
            double desiredWidth = Math.Max(desiredHeight * 2, 5 * _message.Length / newLineCount);
            while (desiredWidth > 2 * desiredHeight)
            {
                desiredHeight *= 1.05;
                desiredWidth *= 0.95;
            }
            this.Width = Math.Max(300, desiredWidth);
            this.Height = Math.Max(100, desiredHeight);
            this.UpdateLayout(); // Áp dụng cuối, nếu desired height > max, scroll sẽ xuất hiện
            EnsureProperCentering();
        }

        private void EnsureProperCentering()
        {
            try
            {
                // If we have an owner, manually center the window
                if (this.Owner != null && this.Owner.IsLoaded)
                {
                    // Calculate center position relative to owner
                    double centerX = this.Owner.Left + (this.Owner.Width - this.Width) / 2;
                    double centerY = this.Owner.Top + (this.Owner.Height - this.Height) / 2;

                    // Get the monitor where the owner is located
                    System.Drawing.Point ownerCenter = new System.Drawing.Point(
                        (int)(this.Owner.Left + this.Owner.Width / 2),
                        (int)(this.Owner.Top + this.Owner.Height / 2)
                    );
                    IntPtr hMonitor = MonitorFromPoint(ownerCenter, MONITOR_DEFAULTTONEAREST);

                    MONITORINFO mi = new MONITORINFO();
                    mi.cbSize = Marshal.SizeOf(mi);
                    if (GetMonitorInfo(hMonitor, ref mi))
                    {
                        RECT workArea = mi.rcWork;
                        double monitorLeft = workArea.Left;
                        double monitorTop = workArea.Top;
                        double monitorWidth = workArea.Right - workArea.Left;
                        double monitorHeight = workArea.Bottom - workArea.Top;

                        // Clamp to the monitor's working area
                        double minX = monitorLeft;
                        double minY = monitorTop;
                        double maxX = monitorLeft + monitorWidth - this.Width;
                        double maxY = monitorTop + monitorHeight - this.Height;

                        centerX = Math.Max(minX, Math.Min(maxX, centerX));
                        centerY = Math.Max(minY, Math.Min(maxY, centerY));
                    }
                    // If GetMonitorInfo fails, fallback to primary screen (rare case)
                    else
                    {
                        double maxX = SystemParameters.PrimaryScreenWidth - this.Width;
                        double maxY = SystemParameters.PrimaryScreenHeight - this.Height;
                        double minX = 0;
                        double minY = 0;

                        centerX = Math.Max(minX, Math.Min(maxX, centerX));
                        centerY = Math.Max(minY, Math.Min(maxY, centerY));
                    }

                    this.Left = centerX;
                    this.Top = centerY;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to manually center notification window: {ex.Message}");
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
                    case Key.Tab:
                        // Xử lý tab navigation với hiệu ứng visual
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            // Shift+Tab: di chuyển ngược lại
                            if (OKButton.IsFocused)
                            {
                                CancelButton.Focus();
                                e.Handled = true;
                            }
                            else if (CancelButton.IsFocused)
                            {
                                OKButton.Focus();
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            // Tab: di chuyển tiến lên
                            if (OKButton.IsFocused)
                            {
                                CancelButton.Focus();
                                e.Handled = true;
                            }
                            else if (CancelButton.IsFocused)
                            {
                                OKButton.Focus();
                                e.Handled = true;
                            }
                            else
                            {
                                OKButton.Focus();
                                e.Handled = true;
                            }
                        }
                        break;
                    case Key.Left:
                    case Key.Up:
                        // Di chuyển focus sang button trước đó
                        if (CancelButton.IsFocused)
                        {
                            OKButton.Focus();
                        }
                        else
                        {
                            CancelButton.Focus();
                        }
                        e.Handled = true;
                        break;
                    case Key.Right:
                    case Key.Down:
                        // Di chuyển focus sang button tiếp theo
                        if (OKButton.IsFocused)
                        {
                            CancelButton.Focus();
                        }
                        else
                        {
                            OKButton.Focus();
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