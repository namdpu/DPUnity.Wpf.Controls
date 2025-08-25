using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DPUnity.Wpf.Controls.Controls.ColorPickers
{
    public partial class ColorPicker : UserControl
    {
        private bool _isUpdatingColor = false;
        private bool _isDragging = false;
        private bool _isRgbaMode = true; // Mặc định là RGBA

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new PropertyMetadata(Colors.Black, OnSelectedColorChanged));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // DefaultColors are always taken from ACItoRGBLookup - no external binding allowed
        public List<Color> DefaultColors
        {
            get { return ACItoRGBLookup().Take(15).Select(t => t.Item2).ToList(); }
        }

        // RecentColors are now managed by the static RecentColorsManager
        public IReadOnlyList<Color> RecentColors
        {
            get { return RecentColorsManager.RecentColors; }
        }

        public static readonly DependencyProperty SelectedColorIndexProperty =
            DependencyProperty.Register("SelectedColorIndex", typeof(int?), typeof(ColorPicker),
                new PropertyMetadata(0, OnSelectedColorIndexChanged));

        public int? SelectedColorIndex
        {
            get { return (int?)GetValue(SelectedColorIndexProperty); }
            set { SetValue(SelectedColorIndexProperty, value); }
        }

        public event EventHandler<Color>? ColorChanged;
        public event EventHandler? Confirmed;
        public event EventHandler? Cancelled;

        private double _hue = 0;
        private double _saturation = 1;
        private double _brightness = 1;
        private double _alpha = 1;

        private Canvas? colorCanvas;
        private Ellipse? colorSelector;
        private Canvas? hueCanvas;
        private Rectangle? hueSelector;
        private Canvas? alphaCanvas;
        private Rectangle? alphaSelector;
        private GradientStop? alphaGradientTop;
        private GradientStop? alphaGradientBottom;
        private Rectangle? currentColorDisplay;
        private TextBox? redTextBox;
        private TextBox? greenTextBox;
        private TextBox? blueTextBox;
        private TextBox? alphaTextBox;
        private TextBox? hexTextBox;
        private WrapPanel? defaultColorsPanel;
        private WrapPanel? recentColorsPanel;
        private Grid? rgbaGrid;
        private Grid? hexGrid;
        private Button? toggleModeButton;
        private HandyControl.Controls.NumericUpDown? colorIndexUpDown;

        public ColorPicker()
        {
            InitializeComponent();
            InitializeDefaultColors();

            colorCanvas = FindName("ColorCanvas") as Canvas;
            colorSelector = FindName("ColorSelector") as Ellipse;
            hueCanvas = FindName("HueCanvas") as Canvas;
            hueSelector = FindName("HueSelector") as Rectangle;
            alphaCanvas = FindName("AlphaCanvas") as Canvas;
            alphaSelector = FindName("AlphaSelector") as Rectangle;
            alphaGradientTop = FindName("AlphaGradientTop") as GradientStop;
            alphaGradientBottom = FindName("AlphaGradientBottom") as GradientStop;
            currentColorDisplay = FindName("CurrentColorDisplay") as Rectangle;
            redTextBox = FindName("RedTextBox") as TextBox;
            greenTextBox = FindName("GreenTextBox") as TextBox;
            blueTextBox = FindName("BlueTextBox") as TextBox;
            alphaTextBox = FindName("AlphaTextBox") as TextBox;
            hexTextBox = FindName("HexTextBox") as TextBox;
            defaultColorsPanel = FindName("DefaultColorsPanel") as WrapPanel;
            recentColorsPanel = FindName("RecentColorsPanel") as WrapPanel;
            rgbaGrid = FindName("RgbaGrid") as Grid;
            hexGrid = FindName("HexGrid") as Grid;
            toggleModeButton = FindName("ToggleModeButton") as Button;
            colorIndexUpDown = FindName("ColorIndexUpDown") as HandyControl.Controls.NumericUpDown;

            // Đăng ký event handler cho RecentColorsManager
            RecentColorsManager.RecentColorsChanged += OnRecentColorsChanged;

            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }

        private void InitializeDefaultColors()
        {
            // DefaultColors are now always from ACItoRGBLookup, no need to set
            // RecentColors are managed by the static RecentColorsManager
        }

        /// <summary>
        /// Event handler cho khi danh sách recent colors thay đổi
        /// </summary>
        private void OnRecentColorsChanged(object? sender, EventArgs e)
        {
            // Update UI trên UI thread
            if (Dispatcher.CheckAccess())
            {
                UpdateRecentColorsPanel();
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => UpdateRecentColorsPanel()));
            }
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = d as ColorPicker;
            colorPicker?.UpdateFromSelectedColor();
        }

        private static void OnSelectedColorIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = d as ColorPicker;
            colorPicker?.UpdateFromSelectedColorIndex();
        }

        private void UpdateFromSelectedColor()
        {
            if (_isUpdatingColor || !IsLoaded) return;

            _isUpdatingColor = true;

            var color = SelectedColor;
            ColorToHsb(color, out _hue, out _saturation, out _brightness);
            _alpha = color.A / 255.0;

            UpdateColorCanvas();
            UpdateHueSlider();
            UpdateAlphaSlider();
            UpdateTextBoxes();
            UpdateCurrentColorDisplay();
            UpdateIndexFromColor();

            _isUpdatingColor = false;
        }

        private void UpdateFromSelectedColorIndex()
        {
            if (_isUpdatingColor) return;

            if (SelectedColorIndex.HasValue)
            {
                var color = GetColorByIndex(SelectedColorIndex.Value);
                if (color.HasValue)
                {
                    _isUpdatingColor = true;

                    SelectedColor = color.Value;
                    ColorToHsb(color.Value, out _hue, out _saturation, out _brightness);
                    _alpha = color.Value.A / 255.0;

                    // Only update UI if control is loaded, otherwise it will be updated in UserControl_Loaded
                    if (IsLoaded)
                    {
                        UpdateColorCanvas();
                        UpdateHueSlider();
                        UpdateAlphaSlider();
                        UpdateTextBoxes();
                        UpdateCurrentColorDisplay();

                        // Update the NumericUpDown to reflect the index
                        if (colorIndexUpDown != null)
                        {
                            colorIndexUpDown.Value = SelectedColorIndex.Value;
                        }
                    }

                    _isUpdatingColor = false;
                }
            }
        }

        private void UpdateColorCanvas()
        {
            if (colorCanvas == null || colorSelector == null) return;

            // Update canvas background color
            var hueColor = HsbToColor(_hue, 1, 1, 255);
            colorCanvas.Background = new SolidColorBrush(hueColor);

            // Update selector position
            var x = _saturation * colorCanvas.ActualWidth - 5;
            var y = (1 - _brightness) * colorCanvas.ActualHeight - 5;

            Canvas.SetLeft(colorSelector, Math.Max(0, Math.Min(x, colorCanvas.ActualWidth - 10)));
            Canvas.SetTop(colorSelector, Math.Max(0, Math.Min(y, colorCanvas.ActualHeight - 10)));
        }

        private void UpdateHueSlider()
        {
            if (hueCanvas == null || hueSelector == null) return;

            var y = _hue / 360.0 * hueCanvas.ActualHeight - 2;
            Canvas.SetTop(hueSelector, Math.Max(0, Math.Min(y, hueCanvas.ActualHeight - 4)));
        }

        private void UpdateAlphaSlider()
        {
            if (alphaCanvas == null || alphaSelector == null || alphaGradientTop == null || alphaGradientBottom == null) return;

            var selectedColor = HsbToColor(_hue, _saturation, _brightness, 255);
            alphaGradientTop.Color = selectedColor;
            alphaGradientBottom.Color = Color.FromArgb(0, selectedColor.R, selectedColor.G, selectedColor.B);

            var y = (1 - _alpha) * alphaCanvas.ActualHeight - 2;
            Canvas.SetTop(alphaSelector, Math.Max(0, Math.Min(y, alphaCanvas.ActualHeight - 4)));
        }

        private void UpdateTextBoxes()
        {
            if (_isRgbaMode)
            {
                if (redTextBox == null || greenTextBox == null || blueTextBox == null || alphaTextBox == null) return;

                var color = HsbToColor(_hue, _saturation, _brightness, (byte)Math.Round(_alpha * 255));

                redTextBox.Text = color.R.ToString();
                greenTextBox.Text = color.G.ToString();
                blueTextBox.Text = color.B.ToString();
                alphaTextBox.Text = color.A.ToString();
            }
            else
            {
                if (hexTextBox == null) return;

                var color = HsbToColor(_hue, _saturation, _brightness, (byte)Math.Round(_alpha * 255));
                hexTextBox.Text = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
        }

        private void UpdateCurrentColorDisplay()
        {
            if (currentColorDisplay == null) return;

            var color = HsbToColor(_hue, _saturation, _brightness, (byte)Math.Round(_alpha * 255));
            currentColorDisplay.Fill = new SolidColorBrush(color);
            SelectedColor = color;
        }

        private void UpdateDefaultColorsPanel()
        {
            if (defaultColorsPanel == null) return;

            defaultColorsPanel.Children.Clear();

            // DefaultColors is always available from ACItoRGBLookup
            foreach (var color in DefaultColors)
            {
                var button = CreateColorButton(color);
                defaultColorsPanel.Children.Add(button);
            }
        }

        private void UpdateRecentColorsPanel()
        {
            if (recentColorsPanel == null) return;

            recentColorsPanel.Children.Clear();

            foreach (var color in RecentColors)
            {
                var button = CreateColorButton(color);
                recentColorsPanel.Children.Add(button);
            }
        }

        private Button CreateColorButton(Color color)
        {
            var button = new Button
            {
                Background = new SolidColorBrush(color),
                Style = (Style)FindResource("ColorSwatchStyle")
            };

            button.Click += (s, e) =>
            {
                // Mark event as handled to prevent DataGrid from interfering
                e.Handled = true;
                
                // Find the exact index for this color from ACItoRGBLookup
                var exactIndex = FindIndexByColor(color);
                
                if (exactIndex.HasValue)
                {
                    // If we found exact match, set both color and index
                    _isUpdatingColor = true;
                    SelectedColor = color;
                    SelectedColorIndex = exactIndex.Value;
                    if (colorIndexUpDown != null)
                    {
                        colorIndexUpDown.Value = exactIndex.Value;
                    }
                    _isUpdatingColor = false;
                    
                    // Update the UI components
                    ColorToHsb(color, out _hue, out _saturation, out _brightness);
                    _alpha = color.A / 255.0;
                    UpdateColorCanvas();
                    UpdateHueSlider();
                    UpdateAlphaSlider();
                    UpdateTextBoxes();
                    UpdateCurrentColorDisplay();
                }
                else
                {
                    // Fallback to normal behavior
                    SelectedColor = color;
                    UpdateFromSelectedColor();
                }
                
                // Thêm màu vào danh sách recent colors khi click
                RecentColorsManager.AddRecentColor(color);
                
                ColorChanged?.Invoke(this, color);
            };

            return button;
        }

        private void ColorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isDragging = true;
            if (colorCanvas != null) colorCanvas.CaptureMouse();
            UpdateColorFromCanvas(e.GetPosition(colorCanvas));
        }

        private void ColorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true; // Prevent DataGrid from handling this event
                UpdateColorFromCanvas(e.GetPosition(colorCanvas));
            }
        }

        private void ColorCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isDragging = false;
            if (colorCanvas != null) colorCanvas.ReleaseMouseCapture();
            // Update index after mouse is released
            UpdateIndexFromColorOnMouseUp();
        }

        private void UpdateColorFromCanvas(Point position)
        {
            if (_isUpdatingColor || colorCanvas == null) return;

            _saturation = Math.Max(0, Math.Min(1, position.X / colorCanvas.ActualWidth));
            _brightness = Math.Max(0, Math.Min(1, 1 - (position.Y / colorCanvas.ActualHeight)));

            UpdateColorCanvas();
            UpdateAlphaSlider();
            UpdateTextBoxes();
            UpdateCurrentColorDisplay();
            // Don't update index during dragging to avoid lag

            ColorChanged?.Invoke(this, SelectedColor);
        }

        private void HueCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isDragging = true;
            if (hueCanvas != null) hueCanvas.CaptureMouse();
            UpdateHueFromCanvas(e.GetPosition(hueCanvas));
        }

        private void HueCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true; // Prevent DataGrid from handling this event
                UpdateHueFromCanvas(e.GetPosition(hueCanvas));
            }
        }

        private void HueCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isDragging = false;
            if (hueCanvas != null) hueCanvas.ReleaseMouseCapture();
            // Update index after mouse is released
            UpdateIndexFromColorOnMouseUp();
        }

        private void UpdateHueFromCanvas(Point position)
        {
            if (_isUpdatingColor || hueCanvas == null) return;

            _hue = Math.Max(0, Math.Min(360, (position.Y / hueCanvas.ActualHeight) * 360));

            UpdateColorCanvas();
            UpdateHueSlider();
            UpdateAlphaSlider();
            UpdateTextBoxes();
            UpdateCurrentColorDisplay();
            // Don't update index during dragging to avoid lag

            ColorChanged?.Invoke(this, SelectedColor);
        }

        private void AlphaCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isDragging = true;
            if (alphaCanvas != null) alphaCanvas.CaptureMouse();
            UpdateAlphaFromCanvas(e.GetPosition(alphaCanvas));
        }

        private void AlphaCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true; // Prevent DataGrid from handling this event
                UpdateAlphaFromCanvas(e.GetPosition(alphaCanvas));
            }
        }

        private void AlphaCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isDragging = false;
            if (alphaCanvas != null) alphaCanvas.ReleaseMouseCapture();
            // Update index after mouse is released
            UpdateIndexFromColorOnMouseUp();
        }

        private void UpdateAlphaFromCanvas(Point position)
        {
            if (_isUpdatingColor || alphaCanvas == null) return;

            _alpha = Math.Max(0, Math.Min(1, 1 - (position.Y / alphaCanvas.ActualHeight)));

            UpdateAlphaSlider();
            UpdateTextBoxes();
            UpdateCurrentColorDisplay();
            // Don't update index during dragging to avoid lag

            ColorChanged?.Invoke(this, SelectedColor);
        }

        private void ColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingColor) return;

            if (redTextBox == null || greenTextBox == null || blueTextBox == null || alphaTextBox == null) return;

            if (sender is not TextBox textBox) return;
            
            // Mark event as handled to prevent DataGrid interference
            e.Handled = true;

            if (byte.TryParse(redTextBox.Text, out byte r) &&
                byte.TryParse(greenTextBox.Text, out byte g) &&
                byte.TryParse(blueTextBox.Text, out byte b) &&
                byte.TryParse(alphaTextBox.Text, out byte a))
            {
                var color = Color.FromArgb(a, r, g, b);
                ColorToHsb(color, out _hue, out _saturation, out _brightness);
                _alpha = a / 255.0;

                UpdateColorCanvas();
                UpdateHueSlider();
                UpdateAlphaSlider();
                UpdateCurrentColorDisplay();
                UpdateIndexFromColor(); // Update index immediately for text input

                ColorChanged?.Invoke(this, color);
            }
        }

        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingColor || hexTextBox == null) return;
            
            // Mark event as handled to prevent DataGrid interference
            e.Handled = true;

            var hex = hexTextBox.Text.TrimStart('#');
            if (hex.Length == 8 && byte.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out byte a) &&
                byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out byte r) &&
                byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out byte g) &&
                byte.TryParse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b))
            {
                var color = Color.FromArgb(a, r, g, b);
                ColorToHsb(color, out _hue, out _saturation, out _brightness);
                _alpha = a / 255.0;

                UpdateColorCanvas();
                UpdateHueSlider();
                UpdateAlphaSlider();
                UpdateCurrentColorDisplay();
                UpdateIndexFromColor(); // Update index immediately for text input

                ColorChanged?.Invoke(this, color);
            }
        }

        private void ToggleModeButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            _isRgbaMode = !_isRgbaMode;
            if (rgbaGrid != null && hexGrid != null)
            {
                rgbaGrid.Visibility = _isRgbaMode ? Visibility.Visible : Visibility.Collapsed;
                hexGrid.Visibility = _isRgbaMode ? Visibility.Collapsed : Visibility.Visible;
            }
            UpdateTextBoxes();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // Prevent DataGrid from handling this event
            
            // Thêm màu hiện tại vào danh sách recent colors
            RecentColorsManager.AddRecentColor(SelectedColor);

            Confirmed?.Invoke(this, EventArgs.Empty);
        }

        #region HSB Color Conversion

        private static void ColorToHsb(Color color, out double hue, out double saturation, out double brightness)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            brightness = max;
            saturation = max == 0 ? 0 : (max - min) / max;

            if (max == min)
            {
                hue = 0;
            }
            else
            {
                double delta = max - min;
                if (max == r)
                    hue = ((g - b) / delta) % 6;
                else if (max == g)
                    hue = (b - r) / delta + 2;
                else
                    hue = (r - g) / delta + 4;

                hue *= 60;
                if (hue < 0)
                    hue += 360;
            }
        }

        private static Color HsbToColor(double hue, double saturation, double brightness, byte alpha)
        {
            double c = brightness * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = brightness - c;

            double r, g, b;

            if (hue is >= 0 and < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (hue is >= 60 and < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (hue is >= 120 and < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (hue is >= 180 and < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (hue is >= 240 and < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return Color.FromArgb(alpha,
                (byte)Math.Round((r + m) * 255),
                (byte)Math.Round((g + m) * 255),
                (byte)Math.Round((b + m) * 255));
        }

        #endregion

        #region Index Helper Methods

        private static List<Tuple<int, Color>>? _aciLookup;
        private static List<Tuple<int, Color>> GetACILookup()
        {
            return _aciLookup ??= ACItoRGBLookup();
        }

        private int? FindIndexByColor(Color color)
        {
            var lookup = GetACILookup();
            var match = lookup.FirstOrDefault(x => x.Item2.R == color.R &&
                                                  x.Item2.G == color.G &&
                                                  x.Item2.B == color.B &&
                                                  x.Item2.A == color.A);
            return match?.Item1;
        }

        private int FindNearestColorIndex(Color color)
        {
            var lookup = GetACILookup();
            if (!lookup.Any()) return 0;

            double minDistance = double.MaxValue;
            int nearestIndex = 0;

            foreach (var (index, lookupColor) in lookup)
            {
                double distance = CalculateWeightedColorDistance(color, lookupColor);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = index;
                }
            }

            return nearestIndex;
        }

        private static double CalculateWeightedColorDistance(Color color1, Color color2)
        {
            // Sử dụng trọng số để tăng độ chính xác cho mắt người
            // Mắt người nhạy cảm hơn với màu xanh lá cây, sau đó là đỏ, cuối cùng là xanh dương
            const double redWeight = 0.3;
            const double greenWeight = 0.59;
            const double blueWeight = 0.11;
            const double alphaWeight = 0.1;

            double deltaR = color1.R - color2.R;
            double deltaG = color1.G - color2.G;
            double deltaB = color1.B - color2.B;
            double deltaA = color1.A - color2.A;

            return Math.Sqrt(
                redWeight * deltaR * deltaR +
                greenWeight * deltaG * deltaG +
                blueWeight * deltaB * deltaB +
                alphaWeight * deltaA * deltaA
            );
        }

        private Color? GetColorByIndex(int index)
        {
            var lookup = GetACILookup();
            var match = lookup.FirstOrDefault(x => x.Item1 == index);
            return match?.Item2;
        }

        private void UpdateIndexFromColor()
        {
            if (_isUpdatingColor || colorIndexUpDown == null) return;

            var currentColor = HsbToColor(_hue, _saturation, _brightness, (byte)Math.Round(_alpha * 255));
            var exactIndex = FindIndexByColor(currentColor);
            var oldIndex = SelectedColorIndex;

            _isUpdatingColor = true;

            if (exactIndex.HasValue)
            {
                // Exact match found
                colorIndexUpDown.Value = exactIndex.Value;
                SelectedColorIndex = exactIndex.Value;
            }
            else
            {
                // No exact match, find nearest color
                var nearestIndex = FindNearestColorIndex(currentColor);
                colorIndexUpDown.Value = nearestIndex;
                SelectedColorIndex = nearestIndex;
            }

            // Fire event if index changed
            if (oldIndex != SelectedColorIndex)
            {
                // ColorIndexChanged event removed - no longer needed
            }

            _isUpdatingColor = false;
        }

        private void UpdateIndexFromColorOnMouseUp()
        {
            // This method is called only when mouse is released to avoid lag during dragging
            UpdateIndexFromColor();
        }

        #endregion

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (IsLoaded)
            {
                UpdateColorCanvas();
                UpdateHueSlider();
                UpdateAlphaSlider();
                UpdateDefaultColorsPanel();
                UpdateRecentColorsPanel();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure proper focus and input handling for DataGrid scenarios
            this.Focusable = true;
            this.IsTabStop = true;
            
            // Make all textboxes focusable and ensure they can receive input
            if (redTextBox != null)
            {
                redTextBox.Focusable = true;
                redTextBox.IsTabStop = true;
            }
            if (greenTextBox != null)
            {
                greenTextBox.Focusable = true;
                greenTextBox.IsTabStop = true;
            }
            if (blueTextBox != null)
            {
                blueTextBox.Focusable = true;
                blueTextBox.IsTabStop = true;
            }
            if (alphaTextBox != null)
            {
                alphaTextBox.Focusable = true;
                alphaTextBox.IsTabStop = true;
            }
            if (hexTextBox != null)
            {
                hexTextBox.Focusable = true;
                hexTextBox.IsTabStop = true;
            }
            if (colorIndexUpDown != null)
            {
                colorIndexUpDown.Focusable = true;
                colorIndexUpDown.IsTabStop = true;
            }
            
            UpdateDefaultColorsPanel();
            UpdateRecentColorsPanel();
            
            // If SelectedColorIndex was set before loading, apply it now
            if (SelectedColorIndex.HasValue)
            {
                UpdateFromSelectedColorIndex();
            }
            else
            {
                UpdateFromSelectedColor();
                // Ensure initial color index is set correctly for black color (index 0)
                if (SelectedColor == Colors.Black && !SelectedColorIndex.HasValue)
                {
                    SelectedColorIndex = 0;
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Cleanup: unsubscribe từ RecentColorsManager event
            RecentColorsManager.RecentColorsChanged -= OnRecentColorsChanged;
        }

        private static List<Tuple<int, Color>> ACItoRGBLookup()
        {
            List<Tuple<int, Color>> ret = new List<Tuple<int, Color>>();
            ret.Add(Tuple.Create(0, new Color() { A = 255, R = 0, G = 0, B = 0 }));
            ret.Add(Tuple.Create(1, new Color() { A = 255, R = 255, G = 0, B = 0 }));
            ret.Add(Tuple.Create(2, new Color() { A = 255, R = 255, G = 255, B = 0 }));
            ret.Add(Tuple.Create(3, new Color() { A = 255, R = 0, G = 255, B = 0 }));
            ret.Add(Tuple.Create(4, new Color() { A = 255, R = 0, G = 255, B = 255 }));
            ret.Add(Tuple.Create(5, new Color() { A = 255, R = 0, G = 0, B = 255 }));
            ret.Add(Tuple.Create(6, new Color() { A = 255, R = 255, G = 0, B = 255 }));
            ret.Add(Tuple.Create(7, new Color() { A = 255, R = 255, G = 255, B = 255 }));
            ret.Add(Tuple.Create(8, new Color() { A = 255, R = 128, G = 128, B = 128 }));
            ret.Add(Tuple.Create(9, new Color() { A = 255, R = 192, G = 192, B = 192 }));
            ret.Add(Tuple.Create(10, new Color() { A = 255, R = 255, G = 0, B = 0 }));
            ret.Add(Tuple.Create(11, new Color() { A = 255, R = 255, G = 127, B = 127 }));
            ret.Add(Tuple.Create(12, new Color() { A = 255, R = 165, G = 0, B = 0 }));
            ret.Add(Tuple.Create(13, new Color() { A = 255, R = 165, G = 82, B = 82 }));
            ret.Add(Tuple.Create(14, new Color() { A = 255, R = 127, G = 0, B = 0 }));
            ret.Add(Tuple.Create(15, new Color() { A = 255, R = 127, G = 63, B = 63 }));
            ret.Add(Tuple.Create(16, new Color() { A = 255, R = 76, G = 0, B = 0 }));
            ret.Add(Tuple.Create(17, new Color() { A = 255, R = 76, G = 38, B = 38 }));
            ret.Add(Tuple.Create(18, new Color() { A = 255, R = 38, G = 0, B = 0 }));
            ret.Add(Tuple.Create(19, new Color() { A = 255, R = 38, G = 19, B = 19 }));
            ret.Add(Tuple.Create(20, new Color() { A = 255, R = 255, G = 63, B = 0 }));
            ret.Add(Tuple.Create(21, new Color() { A = 255, R = 255, G = 159, B = 127 }));
            ret.Add(Tuple.Create(22, new Color() { A = 255, R = 165, G = 41, B = 0 }));
            ret.Add(Tuple.Create(23, new Color() { A = 255, R = 165, G = 103, B = 82 }));
            ret.Add(Tuple.Create(24, new Color() { A = 255, R = 127, G = 31, B = 0 }));
            ret.Add(Tuple.Create(25, new Color() { A = 255, R = 127, G = 79, B = 63 }));
            ret.Add(Tuple.Create(26, new Color() { A = 255, R = 76, G = 19, B = 0 }));
            ret.Add(Tuple.Create(27, new Color() { A = 255, R = 76, G = 47, B = 38 }));
            ret.Add(Tuple.Create(28, new Color() { A = 255, R = 38, G = 9, B = 0 }));
            ret.Add(Tuple.Create(29, new Color() { A = 255, R = 38, G = 23, B = 19 }));
            ret.Add(Tuple.Create(30, new Color() { A = 255, R = 255, G = 127, B = 0 }));
            ret.Add(Tuple.Create(31, new Color() { A = 255, R = 255, G = 191, B = 127 }));
            ret.Add(Tuple.Create(32, new Color() { A = 255, R = 165, G = 82, B = 0 }));
            ret.Add(Tuple.Create(33, new Color() { A = 255, R = 165, G = 124, B = 82 }));
            ret.Add(Tuple.Create(34, new Color() { A = 255, R = 127, G = 63, B = 0 }));
            ret.Add(Tuple.Create(35, new Color() { A = 255, R = 127, G = 95, B = 63 }));
            ret.Add(Tuple.Create(36, new Color() { A = 255, R = 76, G = 38, B = 0 }));
            ret.Add(Tuple.Create(37, new Color() { A = 255, R = 76, G = 57, B = 38 }));
            ret.Add(Tuple.Create(38, new Color() { A = 255, R = 38, G = 19, B = 0 }));
            ret.Add(Tuple.Create(39, new Color() { A = 255, R = 38, G = 28, B = 19 }));
            ret.Add(Tuple.Create(40, new Color() { A = 255, R = 255, G = 191, B = 0 }));
            ret.Add(Tuple.Create(41, new Color() { A = 255, R = 255, G = 223, B = 127 }));
            ret.Add(Tuple.Create(42, new Color() { A = 255, R = 165, G = 124, B = 0 }));
            ret.Add(Tuple.Create(43, new Color() { A = 255, R = 165, G = 145, B = 82 }));
            ret.Add(Tuple.Create(44, new Color() { A = 255, R = 127, G = 95, B = 0 }));
            ret.Add(Tuple.Create(45, new Color() { A = 255, R = 127, G = 111, B = 63 }));
            ret.Add(Tuple.Create(46, new Color() { A = 255, R = 76, G = 57, B = 0 }));
            ret.Add(Tuple.Create(47, new Color() { A = 255, R = 76, G = 66, B = 38 }));
            ret.Add(Tuple.Create(48, new Color() { A = 255, R = 38, G = 28, B = 0 }));
            ret.Add(Tuple.Create(49, new Color() { A = 255, R = 38, G = 33, B = 19 }));
            ret.Add(Tuple.Create(50, new Color() { A = 255, R = 255, G = 255, B = 0 }));
            ret.Add(Tuple.Create(51, new Color() { A = 255, R = 255, G = 255, B = 127 }));
            ret.Add(Tuple.Create(52, new Color() { A = 255, R = 165, G = 165, B = 0 }));
            ret.Add(Tuple.Create(53, new Color() { A = 255, R = 165, G = 165, B = 82 }));
            ret.Add(Tuple.Create(54, new Color() { A = 255, R = 127, G = 127, B = 0 }));
            ret.Add(Tuple.Create(55, new Color() { A = 255, R = 127, G = 127, B = 63 }));
            ret.Add(Tuple.Create(56, new Color() { A = 255, R = 76, G = 76, B = 0 }));
            ret.Add(Tuple.Create(57, new Color() { A = 255, R = 76, G = 76, B = 38 }));
            ret.Add(Tuple.Create(58, new Color() { A = 255, R = 38, G = 38, B = 0 }));
            ret.Add(Tuple.Create(59, new Color() { A = 255, R = 38, G = 38, B = 19 }));
            ret.Add(Tuple.Create(60, new Color() { A = 255, R = 191, G = 255, B = 0 }));
            ret.Add(Tuple.Create(61, new Color() { A = 255, R = 223, G = 255, B = 127 }));
            ret.Add(Tuple.Create(62, new Color() { A = 255, R = 124, G = 165, B = 0 }));
            ret.Add(Tuple.Create(63, new Color() { A = 255, R = 145, G = 165, B = 82 }));
            ret.Add(Tuple.Create(64, new Color() { A = 255, R = 95, G = 127, B = 0 }));
            ret.Add(Tuple.Create(65, new Color() { A = 255, R = 111, G = 127, B = 63 }));
            ret.Add(Tuple.Create(66, new Color() { A = 255, R = 57, G = 76, B = 0 }));
            ret.Add(Tuple.Create(67, new Color() { A = 255, R = 66, G = 76, B = 38 }));
            ret.Add(Tuple.Create(68, new Color() { A = 255, R = 28, G = 38, B = 0 }));
            ret.Add(Tuple.Create(69, new Color() { A = 255, R = 33, G = 38, B = 19 }));
            ret.Add(Tuple.Create(70, new Color() { A = 255, R = 127, G = 255, B = 0 }));
            ret.Add(Tuple.Create(71, new Color() { A = 255, R = 191, G = 255, B = 127 }));
            ret.Add(Tuple.Create(72, new Color() { A = 255, R = 82, G = 165, B = 0 }));
            ret.Add(Tuple.Create(73, new Color() { A = 255, R = 124, G = 165, B = 82 }));
            ret.Add(Tuple.Create(74, new Color() { A = 255, R = 63, G = 127, B = 0 }));
            ret.Add(Tuple.Create(75, new Color() { A = 255, R = 95, G = 127, B = 63 }));
            ret.Add(Tuple.Create(76, new Color() { A = 255, R = 38, G = 76, B = 0 }));
            ret.Add(Tuple.Create(77, new Color() { A = 255, R = 57, G = 76, B = 38 }));
            ret.Add(Tuple.Create(78, new Color() { A = 255, R = 19, G = 38, B = 0 }));
            ret.Add(Tuple.Create(79, new Color() { A = 255, R = 28, G = 38, B = 19 }));
            ret.Add(Tuple.Create(80, new Color() { A = 255, R = 63, G = 255, B = 0 }));
            ret.Add(Tuple.Create(81, new Color() { A = 255, R = 159, G = 255, B = 127 }));
            ret.Add(Tuple.Create(82, new Color() { A = 255, R = 41, G = 165, B = 0 }));
            ret.Add(Tuple.Create(83, new Color() { A = 255, R = 103, G = 165, B = 82 }));
            ret.Add(Tuple.Create(84, new Color() { A = 255, R = 31, G = 127, B = 0 }));
            ret.Add(Tuple.Create(85, new Color() { A = 255, R = 79, G = 127, B = 63 }));
            ret.Add(Tuple.Create(86, new Color() { A = 255, R = 19, G = 76, B = 0 }));
            ret.Add(Tuple.Create(87, new Color() { A = 255, R = 47, G = 76, B = 38 }));
            ret.Add(Tuple.Create(88, new Color() { A = 255, R = 9, G = 38, B = 0 }));
            ret.Add(Tuple.Create(89, new Color() { A = 255, R = 23, G = 38, B = 19 }));
            ret.Add(Tuple.Create(90, new Color() { A = 255, R = 0, G = 255, B = 0 }));
            ret.Add(Tuple.Create(91, new Color() { A = 255, R = 127, G = 255, B = 127 }));
            ret.Add(Tuple.Create(92, new Color() { A = 255, R = 0, G = 165, B = 0 }));
            ret.Add(Tuple.Create(93, new Color() { A = 255, R = 82, G = 165, B = 82 }));
            ret.Add(Tuple.Create(94, new Color() { A = 255, R = 0, G = 127, B = 0 }));
            ret.Add(Tuple.Create(95, new Color() { A = 255, R = 63, G = 127, B = 63 }));
            ret.Add(Tuple.Create(96, new Color() { A = 255, R = 0, G = 76, B = 0 }));
            ret.Add(Tuple.Create(97, new Color() { A = 255, R = 38, G = 76, B = 38 }));
            ret.Add(Tuple.Create(98, new Color() { A = 255, R = 0, G = 38, B = 0 }));
            ret.Add(Tuple.Create(99, new Color() { A = 255, R = 19, G = 38, B = 19 }));
            ret.Add(Tuple.Create(100, new Color() { A = 255, R = 0, G = 255, B = 63 }));
            ret.Add(Tuple.Create(101, new Color() { A = 255, R = 127, G = 255, B = 159 }));
            ret.Add(Tuple.Create(102, new Color() { A = 255, R = 0, G = 165, B = 41 }));
            ret.Add(Tuple.Create(103, new Color() { A = 255, R = 82, G = 165, B = 103 }));
            ret.Add(Tuple.Create(104, new Color() { A = 255, R = 0, G = 127, B = 31 }));
            ret.Add(Tuple.Create(105, new Color() { A = 255, R = 63, G = 127, B = 79 }));
            ret.Add(Tuple.Create(106, new Color() { A = 255, R = 0, G = 76, B = 19 }));
            ret.Add(Tuple.Create(107, new Color() { A = 255, R = 38, G = 76, B = 47 }));
            ret.Add(Tuple.Create(108, new Color() { A = 255, R = 0, G = 38, B = 9 }));
            ret.Add(Tuple.Create(109, new Color() { A = 255, R = 19, G = 38, B = 23 }));
            ret.Add(Tuple.Create(110, new Color() { A = 255, R = 0, G = 255, B = 127 }));
            ret.Add(Tuple.Create(111, new Color() { A = 255, R = 127, G = 255, B = 191 }));
            ret.Add(Tuple.Create(112, new Color() { A = 255, R = 0, G = 165, B = 82 }));
            ret.Add(Tuple.Create(113, new Color() { A = 255, R = 82, G = 165, B = 124 }));
            ret.Add(Tuple.Create(114, new Color() { A = 255, R = 0, G = 127, B = 63 }));
            ret.Add(Tuple.Create(115, new Color() { A = 255, R = 63, G = 127, B = 95 }));
            ret.Add(Tuple.Create(116, new Color() { A = 255, R = 0, G = 76, B = 38 }));
            ret.Add(Tuple.Create(117, new Color() { A = 255, R = 38, G = 76, B = 57 }));
            ret.Add(Tuple.Create(118, new Color() { A = 255, R = 0, G = 38, B = 19 }));
            ret.Add(Tuple.Create(119, new Color() { A = 255, R = 19, G = 38, B = 28 }));
            ret.Add(Tuple.Create(120, new Color() { A = 255, R = 0, G = 255, B = 191 }));
            ret.Add(Tuple.Create(121, new Color() { A = 255, R = 127, G = 255, B = 223 }));
            ret.Add(Tuple.Create(122, new Color() { A = 255, R = 0, G = 165, B = 124 }));
            ret.Add(Tuple.Create(123, new Color() { A = 255, R = 82, G = 165, B = 145 }));
            ret.Add(Tuple.Create(124, new Color() { A = 255, R = 0, G = 127, B = 95 }));
            ret.Add(Tuple.Create(125, new Color() { A = 255, R = 63, G = 127, B = 111 }));
            ret.Add(Tuple.Create(126, new Color() { A = 255, R = 0, G = 76, B = 57 }));
            ret.Add(Tuple.Create(127, new Color() { A = 255, R = 38, G = 76, B = 66 }));
            ret.Add(Tuple.Create(128, new Color() { A = 255, R = 0, G = 38, B = 28 }));
            ret.Add(Tuple.Create(129, new Color() { A = 255, R = 19, G = 38, B = 33 }));
            ret.Add(Tuple.Create(130, new Color() { A = 255, R = 0, G = 255, B = 255 }));
            ret.Add(Tuple.Create(131, new Color() { A = 255, R = 127, G = 255, B = 255 }));
            ret.Add(Tuple.Create(132, new Color() { A = 255, R = 0, G = 165, B = 165 }));
            ret.Add(Tuple.Create(133, new Color() { A = 255, R = 82, G = 165, B = 165 }));
            ret.Add(Tuple.Create(134, new Color() { A = 255, R = 0, G = 127, B = 127 }));
            ret.Add(Tuple.Create(135, new Color() { A = 255, R = 63, G = 127, B = 127 }));
            ret.Add(Tuple.Create(136, new Color() { A = 255, R = 0, G = 76, B = 76 }));
            ret.Add(Tuple.Create(137, new Color() { A = 255, R = 38, G = 76, B = 76 }));
            ret.Add(Tuple.Create(138, new Color() { A = 255, R = 0, G = 38, B = 38 }));
            ret.Add(Tuple.Create(139, new Color() { A = 255, R = 19, G = 38, B = 38 }));
            ret.Add(Tuple.Create(140, new Color() { A = 255, R = 0, G = 191, B = 255 }));
            ret.Add(Tuple.Create(141, new Color() { A = 255, R = 127, G = 223, B = 255 }));
            ret.Add(Tuple.Create(142, new Color() { A = 255, R = 0, G = 124, B = 165 }));
            ret.Add(Tuple.Create(143, new Color() { A = 255, R = 82, G = 145, B = 165 }));
            ret.Add(Tuple.Create(144, new Color() { A = 255, R = 0, G = 95, B = 127 }));
            ret.Add(Tuple.Create(145, new Color() { A = 255, R = 63, G = 111, B = 127 }));
            ret.Add(Tuple.Create(146, new Color() { A = 255, R = 0, G = 57, B = 76 }));
            ret.Add(Tuple.Create(147, new Color() { A = 255, R = 38, G = 66, B = 76 }));
            ret.Add(Tuple.Create(148, new Color() { A = 255, R = 0, G = 28, B = 38 }));
            ret.Add(Tuple.Create(149, new Color() { A = 255, R = 19, G = 33, B = 38 }));
            ret.Add(Tuple.Create(150, new Color() { A = 255, R = 0, G = 127, B = 255 }));
            ret.Add(Tuple.Create(151, new Color() { A = 255, R = 127, G = 191, B = 255 }));
            ret.Add(Tuple.Create(152, new Color() { A = 255, R = 0, G = 82, B = 165 }));
            ret.Add(Tuple.Create(153, new Color() { A = 255, R = 82, G = 124, B = 165 }));
            ret.Add(Tuple.Create(154, new Color() { A = 255, R = 0, G = 63, B = 127 }));
            ret.Add(Tuple.Create(155, new Color() { A = 255, R = 63, G = 95, B = 127 }));
            ret.Add(Tuple.Create(156, new Color() { A = 255, R = 0, G = 38, B = 76 }));
            ret.Add(Tuple.Create(157, new Color() { A = 255, R = 38, G = 57, B = 76 }));
            ret.Add(Tuple.Create(158, new Color() { A = 255, R = 0, G = 19, B = 38 }));
            ret.Add(Tuple.Create(159, new Color() { A = 255, R = 19, G = 28, B = 38 }));
            ret.Add(Tuple.Create(160, new Color() { A = 255, R = 0, G = 63, B = 255 }));
            ret.Add(Tuple.Create(161, new Color() { A = 255, R = 127, G = 159, B = 255 }));
            ret.Add(Tuple.Create(162, new Color() { A = 255, R = 0, G = 41, B = 165 }));
            ret.Add(Tuple.Create(163, new Color() { A = 255, R = 82, G = 103, B = 165 }));
            ret.Add(Tuple.Create(164, new Color() { A = 255, R = 0, G = 31, B = 127 }));
            ret.Add(Tuple.Create(165, new Color() { A = 255, R = 63, G = 79, B = 127 }));
            ret.Add(Tuple.Create(166, new Color() { A = 255, R = 0, G = 19, B = 76 }));
            ret.Add(Tuple.Create(167, new Color() { A = 255, R = 38, G = 47, B = 76 }));
            ret.Add(Tuple.Create(168, new Color() { A = 255, R = 0, G = 9, B = 38 }));
            ret.Add(Tuple.Create(169, new Color() { A = 255, R = 19, G = 23, B = 38 }));
            ret.Add(Tuple.Create(170, new Color() { A = 255, R = 0, G = 0, B = 255 }));
            ret.Add(Tuple.Create(171, new Color() { A = 255, R = 127, G = 127, B = 255 }));
            ret.Add(Tuple.Create(172, new Color() { A = 255, R = 0, G = 0, B = 165 }));
            ret.Add(Tuple.Create(173, new Color() { A = 255, R = 82, G = 82, B = 165 }));
            ret.Add(Tuple.Create(174, new Color() { A = 255, R = 0, G = 0, B = 127 }));
            ret.Add(Tuple.Create(175, new Color() { A = 255, R = 63, G = 63, B = 127 }));
            ret.Add(Tuple.Create(176, new Color() { A = 255, R = 0, G = 0, B = 76 }));
            ret.Add(Tuple.Create(177, new Color() { A = 255, R = 38, G = 38, B = 76 }));
            ret.Add(Tuple.Create(178, new Color() { A = 255, R = 0, G = 0, B = 38 }));
            ret.Add(Tuple.Create(179, new Color() { A = 255, R = 19, G = 19, B = 38 }));
            ret.Add(Tuple.Create(180, new Color() { A = 255, R = 63, G = 0, B = 255 }));
            ret.Add(Tuple.Create(181, new Color() { A = 255, R = 159, G = 127, B = 255 }));
            ret.Add(Tuple.Create(182, new Color() { A = 255, R = 41, G = 0, B = 165 }));
            ret.Add(Tuple.Create(183, new Color() { A = 255, R = 103, G = 82, B = 165 }));
            ret.Add(Tuple.Create(184, new Color() { A = 255, R = 31, G = 0, B = 127 }));
            ret.Add(Tuple.Create(185, new Color() { A = 255, R = 79, G = 63, B = 127 }));
            ret.Add(Tuple.Create(186, new Color() { A = 255, R = 19, G = 0, B = 76 }));
            ret.Add(Tuple.Create(187, new Color() { A = 255, R = 47, G = 38, B = 76 }));
            ret.Add(Tuple.Create(188, new Color() { A = 255, R = 9, G = 0, B = 38 }));
            ret.Add(Tuple.Create(189, new Color() { A = 255, R = 23, G = 19, B = 38 }));
            ret.Add(Tuple.Create(190, new Color() { A = 255, R = 127, G = 0, B = 255 }));
            ret.Add(Tuple.Create(191, new Color() { A = 255, R = 191, G = 127, B = 255 }));
            ret.Add(Tuple.Create(192, new Color() { A = 255, R = 82, G = 0, B = 165 }));
            ret.Add(Tuple.Create(193, new Color() { A = 255, R = 124, G = 82, B = 165 }));
            ret.Add(Tuple.Create(194, new Color() { A = 255, R = 63, G = 0, B = 127 }));
            ret.Add(Tuple.Create(195, new Color() { A = 255, R = 95, G = 63, B = 127 }));
            ret.Add(Tuple.Create(196, new Color() { A = 255, R = 38, G = 0, B = 76 }));
            ret.Add(Tuple.Create(197, new Color() { A = 255, R = 57, G = 38, B = 76 }));
            ret.Add(Tuple.Create(198, new Color() { A = 255, R = 19, G = 0, B = 38 }));
            ret.Add(Tuple.Create(199, new Color() { A = 255, R = 28, G = 19, B = 38 }));
            ret.Add(Tuple.Create(200, new Color() { A = 255, R = 191, G = 0, B = 255 }));
            ret.Add(Tuple.Create(201, new Color() { A = 255, R = 223, G = 127, B = 255 }));
            ret.Add(Tuple.Create(202, new Color() { A = 255, R = 124, G = 0, B = 165 }));
            ret.Add(Tuple.Create(203, new Color() { A = 255, R = 145, G = 82, B = 165 }));
            ret.Add(Tuple.Create(204, new Color() { A = 255, R = 95, G = 0, B = 127 }));
            ret.Add(Tuple.Create(205, new Color() { A = 255, R = 111, G = 63, B = 127 }));
            ret.Add(Tuple.Create(206, new Color() { A = 255, R = 57, G = 0, B = 76 }));
            ret.Add(Tuple.Create(207, new Color() { A = 255, R = 66, G = 38, B = 76 }));
            ret.Add(Tuple.Create(208, new Color() { A = 255, R = 28, G = 0, B = 38 }));
            ret.Add(Tuple.Create(209, new Color() { A = 255, R = 33, G = 19, B = 38 }));
            ret.Add(Tuple.Create(210, new Color() { A = 255, R = 255, G = 0, B = 255 }));
            ret.Add(Tuple.Create(211, new Color() { A = 255, R = 255, G = 127, B = 255 }));
            ret.Add(Tuple.Create(212, new Color() { A = 255, R = 165, G = 0, B = 165 }));
            ret.Add(Tuple.Create(213, new Color() { A = 255, R = 165, G = 82, B = 165 }));
            ret.Add(Tuple.Create(214, new Color() { A = 255, R = 127, G = 0, B = 127 }));
            ret.Add(Tuple.Create(215, new Color() { A = 255, R = 127, G = 63, B = 127 }));
            ret.Add(Tuple.Create(216, new Color() { A = 255, R = 76, G = 0, B = 76 }));
            ret.Add(Tuple.Create(217, new Color() { A = 255, R = 76, G = 38, B = 76 }));
            ret.Add(Tuple.Create(218, new Color() { A = 255, R = 38, G = 0, B = 38 }));
            ret.Add(Tuple.Create(219, new Color() { A = 255, R = 38, G = 19, B = 38 }));
            ret.Add(Tuple.Create(220, new Color() { A = 255, R = 255, G = 0, B = 191 }));
            ret.Add(Tuple.Create(221, new Color() { A = 255, R = 255, G = 127, B = 223 }));
            ret.Add(Tuple.Create(222, new Color() { A = 255, R = 165, G = 0, B = 124 }));
            ret.Add(Tuple.Create(223, new Color() { A = 255, R = 165, G = 82, B = 145 }));
            ret.Add(Tuple.Create(224, new Color() { A = 255, R = 127, G = 0, B = 95 }));
            ret.Add(Tuple.Create(225, new Color() { A = 255, R = 127, G = 63, B = 111 }));
            ret.Add(Tuple.Create(226, new Color() { A = 255, R = 76, G = 0, B = 57 }));
            ret.Add(Tuple.Create(227, new Color() { A = 255, R = 76, G = 38, B = 66 }));
            ret.Add(Tuple.Create(228, new Color() { A = 255, R = 38, G = 0, B = 28 }));
            ret.Add(Tuple.Create(229, new Color() { A = 255, R = 38, G = 19, B = 33 }));
            ret.Add(Tuple.Create(230, new Color() { A = 255, R = 255, G = 0, B = 127 }));
            ret.Add(Tuple.Create(231, new Color() { A = 255, R = 255, G = 127, B = 191 }));
            ret.Add(Tuple.Create(232, new Color() { A = 255, R = 165, G = 0, B = 82 }));
            ret.Add(Tuple.Create(233, new Color() { A = 255, R = 165, G = 82, B = 124 }));
            ret.Add(Tuple.Create(234, new Color() { A = 255, R = 127, G = 0, B = 63 }));
            ret.Add(Tuple.Create(235, new Color() { A = 255, R = 127, G = 63, B = 95 }));
            ret.Add(Tuple.Create(236, new Color() { A = 255, R = 76, G = 0, B = 38 }));
            ret.Add(Tuple.Create(237, new Color() { A = 255, R = 76, G = 38, B = 57 }));
            ret.Add(Tuple.Create(238, new Color() { A = 255, R = 38, G = 0, B = 19 }));
            ret.Add(Tuple.Create(239, new Color() { A = 255, R = 38, G = 19, B = 28 }));
            ret.Add(Tuple.Create(240, new Color() { A = 255, R = 255, G = 0, B = 63 }));
            ret.Add(Tuple.Create(241, new Color() { A = 255, R = 255, G = 127, B = 159 }));
            ret.Add(Tuple.Create(242, new Color() { A = 255, R = 165, G = 0, B = 41 }));
            ret.Add(Tuple.Create(243, new Color() { A = 255, R = 165, G = 82, B = 103 }));
            ret.Add(Tuple.Create(244, new Color() { A = 255, R = 127, G = 0, B = 31 }));
            ret.Add(Tuple.Create(245, new Color() { A = 255, R = 127, G = 63, B = 79 }));
            ret.Add(Tuple.Create(246, new Color() { A = 255, R = 76, G = 0, B = 19 }));
            ret.Add(Tuple.Create(247, new Color() { A = 255, R = 76, G = 38, B = 47 }));
            ret.Add(Tuple.Create(248, new Color() { A = 255, R = 38, G = 0, B = 9 }));
            ret.Add(Tuple.Create(249, new Color() { A = 255, R = 38, G = 19, B = 23 }));
            ret.Add(Tuple.Create(250, new Color() { A = 255, R = 0, G = 0, B = 0 }));
            ret.Add(Tuple.Create(251, new Color() { A = 255, R = 51, G = 51, B = 51 }));
            ret.Add(Tuple.Create(252, new Color() { A = 255, R = 102, G = 102, B = 102 }));
            ret.Add(Tuple.Create(253, new Color() { A = 255, R = 153, G = 153, B = 153 }));
            ret.Add(Tuple.Create(254, new Color() { A = 255, R = 204, G = 204, B = 204 }));
            ret.Add(Tuple.Create(255, new Color() { A = 255, R = 255, G = 255, B = 255 }));
            return ret;
        }

        private void ColorIndexUpDown_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            if (_isUpdatingColor || colorIndexUpDown == null) return;
            
            // Ensure event handling for DataGrid scenarios
            if (e is RoutedEventArgs routedE)
            {
                routedE.Handled = true;
            }

            var index = (int)e.Info;
            var oldIndex = SelectedColorIndex;

            // Check if index is valid (0-255) and within our lookup range
            if (index is >= 0 and <= 255)
            {
                var color = GetColorByIndex(index);
                if (color.HasValue)
                {
                    _isUpdatingColor = true;

                    SelectedColor = color.Value;
                    SelectedColorIndex = index;
                    ColorToHsb(color.Value, out _hue, out _saturation, out _brightness);
                    _alpha = color.Value.A / 255.0;

                    UpdateColorCanvas();
                    UpdateHueSlider();
                    UpdateAlphaSlider();
                    UpdateTextBoxes();
                    UpdateCurrentColorDisplay();

                    _isUpdatingColor = false;

                    ColorChanged?.Invoke(this, color.Value);

                    // ColorIndexChanged event removed - no longer needed
                }
                else
                {
                    SelectedColorIndex = null;
                    // ColorIndexChanged event removed - no longer needed
                }
            }
            else
            {
                SelectedColorIndex = null;
                // ColorIndexChanged event removed - no longer needed
            }
        }
    }
}