using DPUnity.Wpf.UI.Controls.PackIcon;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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
                LoadResourceDictionaries();
                InitializeComponent();
                _type = type;
                _message = message;
                InitializeBasicUI();
                CalculateAndSetSize();
                EnsureWindowCenter();
                ConfigureByType(title);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing notification window: {ex.Message}");
            }
        }
        private void InitializeBasicUI()
        {
            Message.Text = _message;
            Message.TextWrapping = TextWrapping.Wrap; // Đảm bảo wrap để tính height linh hoạt
                                                      // Add keyboard event handler
            this.KeyDown += NotificationWindow_KeyDown;
            // Add resize event handlers
            this.MouseLeftButtonUp += Window_MouseLeftButtonUp;
            this.MouseMove += Window_MouseMove;
        }
        private void CalculateAndSetSize()
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
            if (_type == NotificationType.Ask)
            {
                desiredHeight += 100; // Tăng thêm chiều cao 100 nếu type là Ask do có nút Yes/No
            }
            this.Width = Math.Max(300, desiredWidth);
            this.Height = Math.Max(100, desiredHeight);
            this.UpdateLayout(); // Áp dụng cuối, nếu desired height > max, scroll sẽ xuất hiện
        }
        private void ConfigureByType(string? title)
        {
            switch (_type)
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
                    // Focus vào button đầu tiên và thiết lập tab navigation (sử dụng Dispatcher để trì hoãn)
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        OKButton.Focus();
                        // Đảm bảo keyboard navigation được kích hoạt
                        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
                    }));
                    break;
            }
        }
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        private void EnsureWindowCenter()
        {
            var helper = new WindowInteropHelper(this);
            if (this.Owner != null)
            {
                this.Left = this.Owner.Left + (this.Owner.Width - this.Width) / 2;
                this.Top = this.Owner.Top + (this.Owner.Height - this.Height) / 2;
            }
            else if (helper.Owner != IntPtr.Zero)
            {
                // Owner là native handle (Win32)
                if (GetWindowRect(helper.Owner, out RECT ownerRect))
                {
                    double ownerWidth = ownerRect.Right - ownerRect.Left;
                    double ownerHeight = ownerRect.Bottom - ownerRect.Top;
                    this.Left = ownerRect.Left + (ownerWidth - this.Width) / 2;
                    this.Top = ownerRect.Top + (ownerHeight - this.Height) / 2;
                    // Clamp vị trí vào màn hình để tránh lệch ra ngoài (tùy chọn)
                    EnsureWindowInScreenBounds();
                }
                else
                {
                    // Fallback nếu không lấy được rect
                    CenterToScreen();
                }
            }
            else
            {
                // Không có owner, center to screen
                CenterToScreen();
            }
        }
        private void EnsureWindowInScreenBounds()
        {
            // Lấy monitor gần nhất
            System.Drawing.Point pt = new System.Drawing.Point((int)this.Left, (int)this.Top);
            IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
            MONITORINFO mi = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
            if (GetMonitorInfo(hMonitor, ref mi))
            {
                // Clamp Left và Top vào work area
                this.Left = Math.Max(mi.rcWork.Left, Math.Min(this.Left, mi.rcWork.Right - this.Width));
                this.Top = Math.Max(mi.rcWork.Top, Math.Min(this.Top, mi.rcWork.Bottom - this.Height));
            }
        }
        private void CenterToScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }
        private static ResourceDictionary DPUDict { get; } = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/DPUnity.WPF.UI;component/Styles/DPUnityResources.xaml")
        };

        private void LoadResourceDictionaries()
        {
            try
            {
                this.Resources.MergedDictionaries.Clear();
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