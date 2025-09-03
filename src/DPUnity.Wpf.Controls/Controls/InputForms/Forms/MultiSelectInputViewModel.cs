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
        private string searchText = string.Empty;

        // Collections to hold all items and filtered items
        private ObservableCollection<IInputObject> _allItems = [];
        private ObservableCollection<IInputObject> _selectedItemsInLeft = [];
        private ObservableCollection<IInputObject> _selectedItemsInRight = [];

        public MultiSelectInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterItems();
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
                var filteredItems = _allItems.Where(x => x.Name.Contains(SearchText)).ToList();
                ItemsSource.Clear();
                foreach (var item in filteredItems)
                {
                    ItemsSource.Add(item);
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
