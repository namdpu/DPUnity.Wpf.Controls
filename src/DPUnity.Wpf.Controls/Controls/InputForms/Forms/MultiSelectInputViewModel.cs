using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;
using System.Collections.ObjectModel;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class MultiSelectInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string inputTitle = string.Empty;
        [ObservableProperty]
        private ObservableCollection<IInputObject> itemsSource = [];
        [ObservableProperty]
        private ObservableCollection<IInputObject> selectedItems = [];
        [ObservableProperty]
        private ObservableCollection<IInputObject> filteredSelectedItems = [];
        [ObservableProperty]
        private string searchText = string.Empty;
        [ObservableProperty]
        private string selectedSearchText = string.Empty;
        // Collections to hold all items and filtered items
        private ObservableCollection<IInputObject> _allItems = [];
        private ObservableCollection<IInputObject> _selectedItemsInLeft = [];
        private ObservableCollection<IInputObject> _selectedItemsInRight = [];

        // Biến theo dõi chiều sắp xếp cho bên trái và bên phải
        private bool _leftIsAscending = true;
        private bool _rightIsAscending = true;

        public MultiSelectInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }
        partial void OnItemsSourceChanged(ObservableCollection<IInputObject> value)
        {
            _allItems.Clear();
            if (value != null)
            {
                foreach (var item in value)
                {
                    _allItems.Add(item);
                }
            }
            FilterItems();
        }
        partial void OnSelectedItemsChanged(ObservableCollection<IInputObject> value)
        {
            FilteredSelectedItems.Clear();
            if (value != null)
            {
                foreach (var item in value)
                {
                    FilteredSelectedItems.Add(item);
                }
            }
            FilterSelectedItems();
        }
        partial void OnSearchTextChanged(string value)
        {
            FilterItems();
        }
        partial void OnSelectedSearchTextChanged(string value)
        {
            FilterSelectedItems();
        }
        private void FilterItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                ItemsSource.Clear();
                foreach (var item in _allItems)
                {
                    ItemsSource.Add(item);
                }
            }
            else
            {
                var filteredItems = _allItems.Where(x => x.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                ItemsSource.Clear();
                foreach (var item in filteredItems)
                {
                    ItemsSource.Add(item);
                }
            }
        }
        private void FilterSelectedItems()
        {
            if (string.IsNullOrWhiteSpace(SelectedSearchText))
            {
                FilteredSelectedItems.Clear();
                foreach (var item in SelectedItems)
                {
                    FilteredSelectedItems.Add(item);
                }
            }
            else
            {
                var filteredItems = SelectedItems.Where(x => x.Name.IndexOf(SelectedSearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                FilteredSelectedItems.Clear();
                foreach (var item in filteredItems)
                {
                    FilteredSelectedItems.Add(item);
                }
            }
        }
        [RelayCommand]
        private void MoveRight()
        {
            var itemsToMove = _selectedItemsInLeft.ToList();
            foreach (var item in itemsToMove)
            {
                ItemsSource.Remove(item);
                _allItems.Remove(item);
                SelectedItems.Add(item);
                _selectedItemsInLeft.Remove(item);
            }
            FilterSelectedItems();
        }
        [RelayCommand]
        private void MoveRightAll()
        {
            var itemsToMove = ItemsSource.ToList();
            foreach (var item in itemsToMove)
            {
                ItemsSource.Remove(item);
                _allItems.Remove(item);
                SelectedItems.Add(item);
            }
            _selectedItemsInLeft.Clear();
            FilterSelectedItems();
        }
        [RelayCommand]
        private void MoveLeft()
        {
            var itemsToMove = _selectedItemsInRight.ToList();
            foreach (var item in itemsToMove)
            {
                SelectedItems.Remove(item);
                _allItems.Add(item);
                _selectedItemsInRight.Remove(item);
            }
            FilterItems(); // Re-apply filter to show moved items
            FilterSelectedItems();
        }
        [RelayCommand]
        private void MoveLeftAll()
        {
            var itemsToMove = SelectedItems.ToList();
            foreach (var item in itemsToMove)
            {
                SelectedItems.Remove(item);
                _allItems.Add(item);
            }
            _selectedItemsInRight.Clear();
            FilterItems(); // Re-apply filter to show moved items
            FilterSelectedItems();
        }
        [RelayCommand]
        private void MoveSingleRight(IInputObject item)
        {
            if (item == null) return;
            ItemsSource.Remove(item);
            _allItems.Remove(item);
            SelectedItems.Add(item);
            FilterSelectedItems();
        }
        [RelayCommand]
        private void MoveSingleLeft(IInputObject item)
        {
            if (item == null) return;
            SelectedItems.Remove(item);
            _allItems.Add(item);
            FilterItems(); // Re-apply filter to show moved item
            FilterSelectedItems();
        }
        [RelayCommand]
        private void Submit()
        {
            OK();
        }
        [RelayCommand]
        private new void Cancel()
        {
            base.Cancel();
        }
        [RelayCommand]
        private void LeftSort()
        {
            var sorted = _leftIsAscending
                ? _allItems.OrderBy(x => x.Name).ToList()
                : _allItems.OrderByDescending(x => x.Name).ToList();
            _allItems.Clear();
            foreach (var item in sorted)
            {
                _allItems.Add(item);
            }
            _leftIsAscending = !_leftIsAscending; // Toggle chiều sắp xếp
            FilterItems();
        }
        [RelayCommand]
        private void RightSort()
        {
            var sorted = _rightIsAscending
                ? SelectedItems.OrderBy(x => x.Name).ToList()
                : SelectedItems.OrderByDescending(x => x.Name).ToList();
            SelectedItems.Clear();
            foreach (var item in sorted)
            {
                SelectedItems.Add(item);
            }
            _rightIsAscending = !_rightIsAscending; // Toggle chiều sắp xếp
            FilterSelectedItems();
        }
        /// <summary>
        /// Handles selection change in the left ListView
        /// </summary>
        /// <param name="selectedItems">Currently selected items in left ListView</param>
        public void OnLeftSelectionChanged(IEnumerable<IInputObject> selectedItems)
        {
            _selectedItemsInLeft.Clear();
            foreach (var item in selectedItems)
            {
                _selectedItemsInLeft.Add(item);
            }
        }
        /// <summary>
        /// Handles selection change in the right ListView
        /// </summary>
        /// <param name="selectedItems">Currently selected items in right ListView</param>
        public void OnRightSelectionChanged(IEnumerable<IInputObject> selectedItems)
        {
            _selectedItemsInRight.Clear();
            foreach (var item in selectedItems)
            {
                _selectedItemsInRight.Add(item);
            }
        }
    }
}