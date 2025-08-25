using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DPUnity.Wpf.Controls.Controls.ProjectSettingsMenus
{
    /// <summary>
    /// Interaction logic for ProjectSettingsMenu.xaml
    /// </summary>
    public partial class ProjectSettingsMenu : UserControl
    {
        private ICollectionView? _itemsView;
        private DispatcherTimer? _searchTimer;
        private const double SEARCH_DELAY_MS = 150;

        public ProjectSettingsMenu()
        {
            InitializeComponent();
            InitializeSearchTimer();
        }

        private void InitializeSearchTimer()
        {
            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(SEARCH_DELAY_MS)
            };
            _searchTimer.Tick += OnSearchTimerTick;
        }

        private void OnSearchTimerTick(object? sender, EventArgs e)
        {
            _searchTimer?.Stop();
            ApplyFilter();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ProjectSettingsMenu),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ProjectSettingsMenu control)
            {
                control.SetupCollectionView();
            }
        }

        private void SetupCollectionView()
        {
            if (ItemsSource != null)
            {
                _itemsView = CollectionViewSource.GetDefaultView(ItemsSource);
                _itemsView.Filter = FilterPredicate;
                _itemsView.Refresh();
                HasItems = FilteredItemsCount > 0;
            }
            else
            {
                _itemsView = null;
                HasItems = false;
            }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(ProjectSettingsMenu),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ProjectSettingsMenu control)
            {
                control.OnSearchTextChangedInternal();
            }
        }

        private void OnSearchTextChangedInternal()
        {
            // Dừng timer hiện tại nếu đang chạy
            _searchTimer?.Stop();

            // Bắt đầu timer mới
            _searchTimer?.Start();
        }

        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register("HasItems", typeof(bool), typeof(ProjectSettingsMenu),
                new PropertyMetadata(true));

        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            set { SetValue(HasItemsProperty, value); }
        }

        private void ApplyFilter()
        {
            if (_itemsView != null)
            {
                _itemsView.Refresh();
                // Cập nhật HasItems để hiển thị thông báo khi không có kết quả
                HasItems = FilteredItemsCount > 0;
            }
            else
            {
                HasItems = false;
            }
        }

        private bool FilterPredicate(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            // Kiểm tra nếu item có property Name
            var nameProperty = item?.GetType().GetProperty("Name");
            if (nameProperty != null)
            {
                var name = nameProperty.GetValue(item)?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }

            var descriptionProperty = item?.GetType().GetProperty("Description");
            if (descriptionProperty != null)
            {
                var description = descriptionProperty.GetValue(item)?.ToString();
                if (!string.IsNullOrWhiteSpace(description))
                {
                    return description.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }

            var itemString = item?.ToString();
            return !string.IsNullOrWhiteSpace(itemString) &&
                   itemString.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void ClearSearch()
        {
            SearchText = string.Empty;
        }

        public int FilteredItemsCount => _itemsView?.Cast<object>().Count() ?? 0;

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(ProjectSettingsMenu),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty AddNewCommandProperty =
            DependencyProperty.Register("AddNewCommand", typeof(ICommand), typeof(ProjectSettingsMenu));

        public ICommand AddNewCommand
        {
            get { return (ICommand)GetValue(AddNewCommandProperty); }
            set { SetValue(AddNewCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(ProjectSettingsMenu));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public static readonly DependencyProperty CurrentProjectNameProperty =
            DependencyProperty.Register(
                "CurrentProjectName",
                typeof(string),
                typeof(ProjectSettingsMenu),
                new FrameworkPropertyMetadata("Chưa chọn dự án", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        public string CurrentProjectName
        {
            get { return (string)GetValue(CurrentProjectNameProperty); }
            set { SetValue(CurrentProjectNameProperty, value); }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SearchText = textBox.Text;
            }
        }
    }

    public class StringToHasValueConverter : IValueConverter
    {
        public static readonly StringToHasValueConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
