using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Wpf.Common.Windows;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DPUnity.Windows
{
    public partial class ProcessViewModel : ViewModelPage, IProcessViewModel
    {
        [ObservableProperty]
        private int value = 0;

        [ObservableProperty]
        private int maximum = 100;

        [ObservableProperty]
        private int percent = 0;

        [ObservableProperty]
        private string header = string.Empty;

        [ObservableProperty]
        private int value2;

        [ObservableProperty]
        private int maximum2;

        [ObservableProperty]
        private int percent2 = 0;

        [ObservableProperty]
        private string header2 = string.Empty;

        [ObservableProperty]
        private Visibility progress2Visibility = Visibility.Collapsed;

        [ObservableProperty]
        private int value3;

        [ObservableProperty]
        private int maximum3;

        [ObservableProperty]
        private int percent3 = 0;

        [ObservableProperty]
        private string header3 = string.Empty;

        private int deltaPercent = 1;
        private int deltaPercent2 = 1;
        private int deltaPercent3 = 1;

        [ObservableProperty]
        private Visibility progress3Visibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility expanderVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string informationProcess;

        [ObservableProperty]
        private bool isCancelled;

        [ObservableProperty]
        private bool isClosing;

        [ObservableProperty]
        private bool isExpanded;

        public Func<bool>? IsCancelledAction { set; get; } = null;


        private readonly Dispatcher _dispatcher;
        private CancellationTokenSource _cancellationTokenSource;
        public CancellationToken CancelToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
        public bool IsCanceled => CancelToken.IsCancellationRequested;
       

        public ProcessViewModel(IWindowService windowService, INavigationService navigationService, string header = "Processing...") : base(windowService, navigationService)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _cancellationTokenSource = new CancellationTokenSource();
            informationProcess = header;
            IsCancelled = false;
            IsClosing = false;
            WindowService.CurrentWindow.Window.Height = CalculateWindowHeight();
        }

        [RelayCommand]
        public void Cancel()
        {
            if (IsCancelledAction != null)
            {
                if (IsCancelledAction())
                {
                    CancelHandle();
                }
            }
            else
            {
                if (MessageBox.Show("Bạn muốn hủy bỏ tiến trình đang thực hiện?", "Thông báo",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    CancelHandle();
                }

            }
        }
        public void CancelHandle()
        {
            InformationProcess = "Đang hủy bỏ tiến trình đang thực hiện...";
            WindowService.CurrentWindow.Title = InformationProcess;
            IsCancelled = true;
            IsClosing = true;
            _cancellationTokenSource?.Cancel();
        }
        public void UpdateTitle(string header)
        {
            _dispatcher.Invoke(() =>
            {
                InformationProcess = header;
                WindowService.CurrentWindow.Title = $" {InformationProcess}";
            }, DispatcherPriority.Background);
        }

        public void UpdateProgress(int value, int maximum = -1)
        {
            if (maximum >= 0 && maximum != Maximum)
            {
                Maximum = maximum;
            }
            var newPercent = (int)((value * 100.0) / Maximum);
            _dispatcher.Invoke(() =>
            {

                if (value >= Maximum)
                {
                    Value = Maximum;
                    Percent = 100;
                }
                else
                {
                    int steppedPercent = (newPercent / deltaPercent) * deltaPercent;
                    if (steppedPercent > Percent)
                    {
                        Percent = steppedPercent;
                    }
                }

                Value = value;
                WindowService.CurrentWindow.Title = $"{InformationProcess}";

                // Reset cấp dưới (Progress2 và Progress3)
                Value2 = 0;
                Maximum2 = 0;
                Percent2 = 0;
                Header2 = string.Empty;
                Value3 = 0;
                Maximum3 = 0;
                Percent3 = 0;
                Header3 = string.Empty;
            }, DispatcherPriority.Background);
        }

        public void UpdateProgress2(int value, int maximum = -1)
        {
            if (maximum >= 0 && maximum != Maximum2)
            {
                Maximum2 = maximum;
            }
            var newPercent = (int)((value * 100.0) / Maximum2);
            _dispatcher.Invoke(() =>
            {

                if (value >= Maximum2)
                {
                    Value2 = Maximum2;
                    Percent2 = 100;
                }
                else
                {
                    int steppedPercent = (newPercent / deltaPercent2) * deltaPercent2;
                    if (steppedPercent > Percent2)
                    {
                        Percent2 = steppedPercent;
                    }
                }

                Value2 = value;

                // Reset cấp dưới (Progress3)
                Value3 = 0;
                Maximum3 = 0;
                Percent3 = 0;
                Header3 = string.Empty;
            }, DispatcherPriority.Background);
        }
        public void UpdateProgress3(int value, int maximum = -1)
        {
            if (maximum >= 0 && maximum != Maximum3)
            {
                Maximum3 = maximum;
            }
            var newPercent = (int)((value * 100.0) / Maximum3);
            _dispatcher.Invoke(() =>
            {

                if (value >= Maximum3)
                {
                    Value3 = Maximum3;
                    Percent3 = 100;
                }
                else
                {
                    int steppedPercent = (newPercent / deltaPercent3) * deltaPercent3;
                    if (steppedPercent > Percent3)
                    {
                        Percent3 = steppedPercent;
                    }
                }

                Value3 = value;
            }, DispatcherPriority.Background);
        }

        public void CloseProcess()
        {
            _dispatcher.Invoke(() =>
            {
                IsClosing = true;
                IsCancelled = true;
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                WindowService.CurrentWindow.CloseWindow();
            }, DispatcherPriority.Background);
        }

        public void UpdateProgress()
        {
            Value++;
            var newPercent = (int)((Value * 100.0) / Maximum);
            _dispatcher.Invoke(() =>
            {

                if (Value >= Maximum)
                {
                    Value = Maximum;
                    Percent = 100;
                }
                else
                {
                    int steppedPercent = (newPercent / deltaPercent) * deltaPercent;
                    if (steppedPercent > Percent)
                    {
                        Percent = steppedPercent;
                    }
                }

                Value2 = 0;
                Maximum2 = 0;
                Percent2 = 0;
                Header2 = string.Empty;
                Value3 = 0;
                Maximum3 = 0;
                Percent3 = 0;
                Header3 = string.Empty;
            }, DispatcherPriority.Background);
        }
        public void UpdateProgress2()
        {
            Value2++;
            var newPercent = (int)((Value2 * 100.0) / Maximum2);
            _dispatcher.Invoke(() =>
            {

                if (Value2 >= Maximum2)
                {
                    Value2 = Maximum2;
                    Percent2 = 100;
                }
                else
                {
                    int steppedPercent = (newPercent / deltaPercent2) * deltaPercent2;
                    if (steppedPercent > Percent2)
                    {
                        Percent2 = steppedPercent;
                    }
                }

                Value3 = 0;
                Maximum3 = 0;
                Percent3 = 0;
                Header3 = string.Empty;

                if (Value2 >= Maximum2 && Maximum2 > 0)
                {
                    Value2 = Maximum2 = 0;
                    Percent2 = 0;
                    Header2 = string.Empty;
                }
            }, DispatcherPriority.Background);
        }
        public void UpdateProgress3()
        {
            Value3++;
            var newPercent = (int)((Value3 * 100.0) / Maximum3);
            _dispatcher.Invoke(() =>
            {

                if (Value3 >= Maximum3)
                {
                    Value3 = Maximum3;
                    Percent3 = 100;
                }
                else
                {
                    int steppedPercent = (newPercent / deltaPercent3) * deltaPercent3;
                    if (steppedPercent > Percent3)
                    {
                        Percent3 = steppedPercent;
                    }
                }

                if (Value3 >= Maximum3 && Maximum3 > 0)
                {
                    Value3 = Maximum3 = 0;
                    Percent3 = 0;
                    Header3 = string.Empty;
                }
            }, DispatcherPriority.Background);
        }

        public void InitializeProgress(string header = "", int max = 100, int deltaPercent = 1)
        {
            _dispatcher.Invoke(() =>
            {
                Value = 0;
                Maximum = max;
                Percent = 0;
                Header = header;
                this.deltaPercent = deltaPercent;
            }, DispatcherPriority.Background);
        }

        public void InitializeProgress2(string header = "", int max = 100, int deltaPercent = 1)
        {
            _dispatcher.Invoke(() =>
            {
                Progress2Visibility = Visibility.Visible;
                Value2 = 0;
                Maximum2 = max;
                Percent2 = 0;
                Header2 = header;
                deltaPercent2 = deltaPercent;
            }, DispatcherPriority.Background);
        }

        public void InitializeProgress3(string header = "", int max = 100, int deltaPercent = 1)
        {
            _dispatcher.Invoke(() =>
            {
                Progress3Visibility = Visibility.Visible;
                Value3 = 0;
                Maximum3 = max;
                Percent3 = 0;
                Header3 = header;
                deltaPercent3 = deltaPercent;
            }, DispatcherPriority.Background);
        }

        partial void OnProgress2VisibilityChanged(Visibility value)
        {
            ExpanderVisibility = (value == Visibility.Visible || Progress3Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        partial void OnProgress3VisibilityChanged(Visibility value)
        {
            ExpanderVisibility = (value == Visibility.Visible || Progress2Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        partial void OnIsExpandedChanged(bool value)
        {
            WindowService.CurrentWindow.Window.Height = CalculateWindowHeight();
        }

        private int CalculateWindowHeight()
        {
            int height = 90;

            if (IsExpanded)
            {
                height += 155;
                if (Progress3Visibility == Visibility.Visible)
                {
                    height += 35;
                }
            }
            else if (ExpanderVisibility == Visibility.Visible)
            {
                height += 25;
            }

            return height;
        }
    }
}