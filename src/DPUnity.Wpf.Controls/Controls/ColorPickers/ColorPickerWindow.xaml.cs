using System.Windows;
using System.Windows.Media;

namespace DPUnity.Wpf.Controls.Controls.ColorPickers
{
    public partial class ColorPickerWindow : Window
    {
        public event EventHandler<Color>? ColorSelected;
        public event EventHandler? ColorSelectionCancelled;

        private Color _initialColor;
        private int? _initialColorIndex;

        public ColorPickerWindow()
        {
            InitializeComponent();

            // Handle window events
            Loaded += ColorPickerWindow_Loaded;
            KeyDown += ColorPickerWindow_KeyDown;
        }

        private void ColorPickerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure initial values are set when window is fully loaded
            SetInitialValues();

            // Focus the color picker control when window loads
            ColorPickerControl.Focus();
        }

        private void SetInitialValues()
        {
            if (ColorPickerControl != null)
            {
                // Force set the values multiple times to ensure they stick
                ColorPickerControl.SelectedColor = _initialColor;
                ColorPickerControl.SelectedColorIndex = _initialColorIndex;

                // Force update the control's internal state
                ColorPickerControl.UpdateLayout();

                // Use Dispatcher to ensure values are set after the control is fully rendered
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ColorPickerControl.SelectedColor = _initialColor;
                    ColorPickerControl.SelectedColorIndex = _initialColorIndex;
                }), System.Windows.Threading.DispatcherPriority.Render);
            }
        }

        private void ColorPickerWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle ESC key to cancel
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                CancelSelection();
            }
        }

        public void ShowColorPicker(Color initialColor, int? initialColorIndex, Window? owner = null)
        {
            // Store initial values
            _initialColor = initialColor;
            _initialColorIndex = initialColorIndex;

            // Set owner if provided
            if (owner != null)
            {
                Owner = owner;
            }

            // Set initial values to color picker immediately if it's already loaded
            if (ColorPickerControl != null)
            {
                ColorPickerControl.SelectedColor = initialColor;
                ColorPickerControl.SelectedColorIndex = initialColorIndex;
            }

            // Also set up a handler for when ColorPickerControl is loaded (backup mechanism)
            if (ColorPickerControl != null && !ColorPickerControl.IsLoaded)
            {
                RoutedEventHandler? loadedHandler = null;
                loadedHandler = (s, e) =>
                {
                    SetInitialValues();
                    if (loadedHandler != null)
                        ColorPickerControl.Loaded -= loadedHandler; // Remove handler after use
                };
                ColorPickerControl.Loaded += loadedHandler;
            }

            // Position window near the owner or at center
            PositionWindow(owner);

            // Ensure values are set before showing
            SetInitialValues();

            // Show window as dialog
            ShowDialog();
        }

        private void PositionWindow(Window? owner)
        {
            if (owner != null)
            {
                // Position relative to owner window
                var ownerCenter = new Point(
                    owner.Left + owner.Width / 2,
                    owner.Top + owner.Height / 2
                );

                Left = ownerCenter.X - Width / 2;
                Top = ownerCenter.Y - Height / 2;

                // Ensure window is within screen bounds
                var screen = SystemParameters.WorkArea;
                if (Left < screen.Left) Left = screen.Left;
                if (Top < screen.Top) Top = screen.Top;
                if (Left + Width > screen.Right) Left = screen.Right - Width;
                if (Top + Height > screen.Bottom) Top = screen.Bottom - Height;
            }
            else
            {
                // Center on screen
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void ColorPicker_Confirmed(object sender, EventArgs e)
        {
            // Get selected color and close window
            var selectedColor = ColorPickerControl.SelectedColor;
            DialogResult = true;
            ColorSelected?.Invoke(this, selectedColor);
            Close();
        }

        private void ColorPicker_Cancelled(object sender, EventArgs e)
        {
            CancelSelection();
        }

        private void CancelSelection()
        {
            DialogResult = false;
            ColorSelectionCancelled?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void ColorPicker_ColorChanged(object sender, Color e)
        {
            // Optional: Handle real-time color changes
            // This can be used for preview functionality
        }

        public Color SelectedColor => ColorPickerControl.SelectedColor;
        public int? SelectedColorIndex => ColorPickerControl.SelectedColorIndex;
    }
}
