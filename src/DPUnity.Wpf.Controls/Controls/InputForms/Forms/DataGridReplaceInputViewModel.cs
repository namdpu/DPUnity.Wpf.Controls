using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Common.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class DataGridReplaceInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string inputTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<DataGridColumn> columnsSource = [];

        [ObservableProperty]
        private ObservableCollection<DataGridColumn> selectedColumns = [];

        [ObservableProperty]
        private ObservableCollection<DataGridColumn> filteredSelectedColumns = [];

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string selectedSearchText = string.Empty;

        [ObservableProperty]
        private string replace = string.Empty;

        [ObservableProperty]
        private string replaceWith = string.Empty;

        // Collections to hold all items and filtered items
        private ObservableCollection<DataGridColumn> _allColumns = [];
        private ObservableCollection<DataGridColumn> _selectedColumnsInLeft = [];
        private ObservableCollection<DataGridColumn> _selectedColumnsInRight = [];

        // Variables for sort direction tracking
        private bool _leftIsAscending = true;
        private bool _rightIsAscending = true;

        public DataGridReplaceInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        partial void OnColumnsSourceChanged(ObservableCollection<DataGridColumn> value)
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

        partial void OnSelectedColumnsChanged(ObservableCollection<DataGridColumn> value)
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
                var filteredItems = _allColumns.Where(x => GetColumnHeader(x).IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
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
                var filteredItems = SelectedColumns.Where(x => GetColumnHeader(x).IndexOf(SelectedSearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                FilteredSelectedColumns.Clear();
                foreach (var item in filteredItems)
                {
                    FilteredSelectedColumns.Add(item);
                }
            }
        }

        private string GetColumnHeader(DataGridColumn column)
        {
            return column.Header?.ToString() ?? string.Empty;
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
            SubmitCommand.NotifyCanExecuteChanged();
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
            SubmitCommand.NotifyCanExecuteChanged();
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
            SubmitCommand.NotifyCanExecuteChanged();
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
            SubmitCommand.NotifyCanExecuteChanged();

        }

        [RelayCommand]
        private void MoveSingleRight(DataGridColumn column)
        {
            if (column == null) return;
            ColumnsSource.Remove(column);
            _allColumns.Remove(column);
            SelectedColumns.Add(column);
            FilterSelectedColumns();
            SubmitCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void MoveSingleLeft(DataGridColumn column)
        {
            if (column == null) return;
            SelectedColumns.Remove(column);
            _allColumns.Add(column);
            FilterColumns(); // Re-apply filter to show moved item
            FilterSelectedColumns();
            SubmitCommand.NotifyCanExecuteChanged();
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
                ? _allColumns.OrderBy(x => GetColumnHeader(x)).ToList()
                : _allColumns.OrderByDescending(x => GetColumnHeader(x)).ToList();
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
                ? SelectedColumns.OrderBy(x => GetColumnHeader(x)).ToList()
                : SelectedColumns.OrderByDescending(x => GetColumnHeader(x)).ToList();
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
        public void OnLeftSelectionChanged(IEnumerable<DataGridColumn> selectedColumns)
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
        public void OnRightSelectionChanged(IEnumerable<DataGridColumn> selectedColumns)
        {
            _selectedColumnsInRight.Clear();
            foreach (var column in selectedColumns)
            {
                _selectedColumnsInRight.Add(column);
            }
        }

        /// <summary>
        /// Initialize the control with list of DataGrid columns
        /// </summary>
        /// <param name="columns">List of DataGridColumn</param>
        public void InitializeColumns(List<DataGridColumn> columns)
        {
            _allColumns.Clear();
            ColumnsSource.Clear();

            if (columns != null)
            {
                foreach (var column in columns)
                {
                    _allColumns.Add(column);
                    ColumnsSource.Add(column);
                }
            }
        }

        [RelayCommand]
        private void KeyDown(EventArgs e)
        {
            if (e is not KeyEventArgs keyEventArgs) return;
            switch (keyEventArgs.Key)
            {
                case Key.Enter:
                    if (CanSubmit())
                    {
                        OK();
                        keyEventArgs.Handled = true;
                    }
                    break;
                case Key.Escape:
                    Cancel();
                    keyEventArgs.Handled = true;
                    break;
            }
        }
    }
}
