using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DPUnity.Wpf.Controls.Controls.ProjectSettingsMenus
{
    public interface IDP_SimpleProjectSetting
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public void Save();
    }

    /// <summary>
    /// Interaction logic for SimpleProjectSettingMenu.xaml
    /// </summary>
    public partial class SimpleProjectSettingMenu : UserControl
    {
        private ICollectionView? _itemsView;
        private DispatcherTimer? _searchTimer;
        private const double SEARCH_DELAY_MS = 150;

        public SimpleProjectSettingMenu()
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SimpleProjectSettingMenu),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SimpleProjectSettingMenu control)
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
            DependencyProperty.Register("SearchText", typeof(string), typeof(SimpleProjectSettingMenu),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SimpleProjectSettingMenu control)
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
            DependencyProperty.Register("HasItems", typeof(bool), typeof(SimpleProjectSettingMenu),
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
                    return name!.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }

            var descriptionProperty = item?.GetType().GetProperty("Description");
            if (descriptionProperty != null)
            {
                var description = descriptionProperty.GetValue(item)?.ToString();
                if (!string.IsNullOrWhiteSpace(description))
                {
                    return description!.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }

            var itemString = item?.ToString();
            return !string.IsNullOrWhiteSpace(itemString) &&
                   itemString!.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
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
                typeof(SimpleProjectSettingMenu),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty AddNewCommandProperty =
            DependencyProperty.Register("AddNewCommand", typeof(ICommand), typeof(SimpleProjectSettingMenu));

        public ICommand AddNewCommand
        {
            get { return (ICommand)GetValue(AddNewCommandProperty); }
            set { SetValue(AddNewCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(SimpleProjectSettingMenu));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        // Bỏ ActivateCommand vì không cần tính năng activate

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(SimpleProjectSettingMenu));

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(SimpleProjectSettingMenu));

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(ICommand), typeof(SimpleProjectSettingMenu));

        public ICommand CopyCommand
        {
            get { return (ICommand)GetValue(CopyCommandProperty); }
            set { SetValue(CopyCommandProperty, value); }
        }

        public static readonly DependencyProperty CurrentProjectNameProperty =
            DependencyProperty.Register(
                "CurrentProjectName",
                typeof(string),
                typeof(SimpleProjectSettingMenu),
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

        // Bỏ ActivateMenuItem_Click vì không cần tính năng activate

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is object item)
            {
                EditCommand?.Execute(item);
            }
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is object item)
            {
                CopyCommand?.Execute(item);
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is object item)
            {
                DeleteCommand?.Execute(item);
            }
        }
    }
}
