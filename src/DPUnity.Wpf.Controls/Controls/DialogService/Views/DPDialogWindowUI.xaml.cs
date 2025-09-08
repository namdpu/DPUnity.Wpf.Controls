using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Controls.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace DPUnity.Wpf.Controls.Controls.DialogService.Views
{
    /// <summary>
    /// Interaction logic for DPDialogWindowUI.xaml
    /// </summary>
    public partial class DPDialogWindowUI : Window, IDPDialogWindow
    {
        private Border? _mainBorder;

        public DPDialogWindowUI(IServiceProvider serviceProvider)
        {
            LoadResourceDictionaries();
            InitializeComponent();
            WindowService = serviceProvider.GetRequiredService<IWindowService>();
            WindowService.Init(serviceProvider, this, true);
            NavigationService = serviceProvider.GetRequiredService<INavigationService>();
            NavigationService.Initialize("MainFrame", this);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainBorder = this.Template.FindName("PART_Border", this) as Border;

            _mainBorder ??= this.Content as Border;
            if (_mainBorder != null)
            {
                _mainBorder.SizeChanged += OnBorderSizeChanged;
                UpdateClip();
            }

        }

        private void OnBorderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClip();
        }

        private void UpdateClip()
        {
            if (_mainBorder?.ActualWidth > 0 && _mainBorder.ActualHeight > 0)
            {
                var rect = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, _mainBorder.ActualWidth, _mainBorder.ActualHeight),
                    RadiusX = 16,
                    RadiusY = 16
                };
                _mainBorder.Clip = rect;
            }
        }


        #region Implementation of IDPWindow
        public INavigationService NavigationService { get; }
        public IWindowService WindowService { get; }

        public Window Window => (System.Windows.Window)this;
        public bool IsDialog { get; private set; } = false;

        public void SetWindowOptions(WindowOptions? windowOptions)
        {
            windowOptions ??= new WindowOptions();
            this.MinWidth = windowOptions.MinWidth;
            this.MinHeight = windowOptions.MinHeight;
            this.Width = windowOptions.Width;
            this.Height = windowOptions.Height;
            this.Title = windowOptions.Title;
            this.ResizeMode = windowOptions.ResizeMode;
            if (windowOptions.WindowOwner != IntPtr.Zero)
            {
                _ = new WindowInteropHelper(this)
                {
                    Owner = windowOptions.WindowOwner
                };
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }
        public virtual void HideLoadingOverlay()
        {
            throw new NotImplementedException("Not using LoadingOverlay");
        }

        public virtual void ShowLoadingOverlay()
        {
            throw new NotImplementedException("Not using LoadingOverlay");
        }

        public void SetShowDialog(bool isDialog)
        {
            IsDialog = isDialog;
            WindowService.SetShowDialog(IsDialog);
        }

        public void CloseWindow()
        {
            if (this.IsLoaded)
            {
                this.Close();
            }
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion
    }
}
