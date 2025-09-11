using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using System.Collections.ObjectModel;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class DataGridReplaceInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string inputTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> columnsSource = [];

        [ObservableProperty]
        private ObservableCollection<string> selectedColumns = [];

        [ObservableProperty]
        private ObservableCollection<string> filteredSelectedColumns = [];

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string selectedSearchText = string.Empty;

        [ObservableProperty]
        private string replace = string.Empty;

        [ObservableProperty]
        private string replaceWith = string.Empty;

        // Collections to hold all items and filtered items
        private ObservableCollection<string> _allColumns = [];
        private ObservableCollection<string> _selectedColumnsInLeft = [];
        private ObservableCollection<string> _selectedColumnsInRight = [];

        // Variables for sort direction tracking
        private bool _leftIsAscending = true;
        private bool _rightIsAscending = true;

        public DataGridReplaceInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        partial void OnColumnsSourceChanged(ObservableCollection<string> value)
        {
            _allColumns.Clear();
            if (value != null)
            {
                foreach (var item in value)
                {
                    _allColumns.Add(item);
                }
            }
            FilterColumns();
        }

        partial void OnSelectedColumnsChanged(ObservableCollection<string> value)
        {
            FilteredSelectedColumns.Clear();
            if (value != null)
            {
                foreach (var item in value)
                {
                    FilteredSelectedColumns.Add(item);
                }
            }
            FilterSelectedColumns();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterColumns();
        }

        partial void OnSelectedSearchTextChanged(string value)
        {
            FilterSelectedColumns();
        }

        partial void OnReplaceChanged(string value)
        {
            SubmitCommand.NotifyCanExecuteChanged();
        }

        private void FilterColumns()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                ColumnsSource.Clear();
                foreach (var item in _allColumns)
                {
                    ColumnsSource.Add(item);
                }
            }
            else
            {
                var filteredItems = _allColumns.Where(x => x.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                ColumnsSource.Clear();
                foreach (var item in filteredItems)
                {
                    ColumnsSource.Add(item);
                }
            }
        }

        private void FilterSelectedColumns()
        {
            if (string.IsNullOrWhiteSpace(SelectedSearchText))
            {
                FilteredSelectedColumns.Clear();
                foreach (var item in SelectedColumns)
                {
                    FilteredSelectedColumns.Add(item);
                }
            }
            else
            {
                var filteredItems = SelectedColumns.Where(x => x.IndexOf(SelectedSearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                FilteredSelectedColumns.Clear();
                foreach (var item in filteredItems)
                {
                    FilteredSelectedColumns.Add(item);
                }
            }
        }

        [RelayCommand]
        private void MoveRight()
        {
            var itemsToMove = _selectedColumnsInLeft.ToList();
            foreach (var item in itemsToMove)
            {
                ColumnsSource.Remove(item);
                _allColumns.Remove(item);
                SelectedColumns.Add(item);
                _selectedColumnsInLeft.Remove(item);
            }
            FilterSelectedColumns();
        }

        [RelayCommand]
        private void MoveRightAll()
        {
            var itemsToMove = ColumnsSource.ToList();
            foreach (var item in itemsToMove)
            {
                ColumnsSource.Remove(item);
                _allColumns.Remove(item);
                SelectedColumns.Add(item);
            }
            _selectedColumnsInLeft.Clear();
            FilterSelectedColumns();
        }

        [RelayCommand]
        private void MoveLeft()
        {
            var itemsToMove = _selectedColumnsInRight.ToList();
            foreach (var item in itemsToMove)
            {
                SelectedColumns.Remove(item);
                _allColumns.Add(item);
                _selectedColumnsInRight.Remove(item);
            }
            FilterColumns(); // Re-apply filter to show moved items
            FilterSelectedColumns();
        }

        [RelayCommand]
        private void MoveLeftAll()
        {
            var itemsToMove = SelectedColumns.ToList();
            foreach (var item in itemsToMove)
            {
                SelectedColumns.Remove(item);
                _allColumns.Add(item);
            }
            _selectedColumnsInRight.Clear();
            FilterColumns(); // Re-apply filter to show moved items
            FilterSelectedColumns();
        }

        [RelayCommand]
        private void MoveSingleRight(string column)
        {
            if (string.IsNullOrEmpty(column)) return;
            ColumnsSource.Remove(column);
            _allColumns.Remove(column);
            SelectedColumns.Add(column);
            FilterSelectedColumns();
        }

        [RelayCommand]
        private void MoveSingleLeft(string column)
        {
            if (string.IsNullOrEmpty(column)) return;
            SelectedColumns.Remove(column);
            _allColumns.Add(column);
            FilterColumns(); // Re-apply filter to show moved item
            FilterSelectedColumns();
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            OK();
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrEmpty(Replace) && SelectedColumns.Count > 0;
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
                ? _allColumns.OrderBy(x => x).ToList()
                : _allColumns.OrderByDescending(x => x).ToList();
            _allColumns.Clear();
            foreach (var item in sorted)
            {
                _allColumns.Add(item);
            }
            _leftIsAscending = !_leftIsAscending; // Toggle sort direction
            FilterColumns();
        }

        [RelayCommand]
        private void RightSort()
        {
            var sorted = _rightIsAscending
                ? SelectedColumns.OrderBy(x => x).ToList()
                : SelectedColumns.OrderByDescending(x => x).ToList();
            SelectedColumns.Clear();
            foreach (var item in sorted)
            {
                SelectedColumns.Add(item);
            }
            _rightIsAscending = !_rightIsAscending; // Toggle sort direction
            FilterSelectedColumns();
        }

        /// <summary>
        /// Handles selection change in the left ListView
        /// </summary>
        /// <param name="selectedColumns">Currently selected columns in left ListView</param>
        public void OnLeftSelectionChanged(IEnumerable<string> selectedColumns)
        {
            _selectedColumnsInLeft.Clear();
            foreach (var column in selectedColumns)
            {
                _selectedColumnsInLeft.Add(column);
            }
        }

        /// <summary>
        /// Handles selection change in the right ListView
        /// </summary>
        /// <param name="selectedColumns">Currently selected columns in right ListView</param>
        public void OnRightSelectionChanged(IEnumerable<string> selectedColumns)
        {
            _selectedColumnsInRight.Clear();
            foreach (var column in selectedColumns)
            {
                _selectedColumnsInRight.Add(column);
            }
        }

        /// <summary>
        /// Initialize the control with list of column names
        /// </summary>
        /// <param name="columnNames">List of column names</param>
        public void InitializeColumns(List<string> columnNames)
        {
            _allColumns.Clear();
            ColumnsSource.Clear();

            if (columnNames != null)
            {
                foreach (var columnName in columnNames)
                {
                    _allColumns.Add(columnName);
                    ColumnsSource.Add(columnName);
                }
            }
        }
    }
}
