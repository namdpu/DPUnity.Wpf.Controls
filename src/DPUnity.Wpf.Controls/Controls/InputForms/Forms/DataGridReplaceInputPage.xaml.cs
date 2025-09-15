using DPUnity.Windows;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for DataGridReplaceInputPage.xaml
    /// </summary>
    public partial class DataGridReplaceInputPage : DPage
    {
        private new DataGridReplaceInputViewModel ViewModel => (DataGridReplaceInputViewModel)DataContext;

        public DataGridReplaceInputPage(DataGridReplaceInputViewModel vm) : base(vm)
        {
            InitializeComponent();
        }

        private void LeftListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var selectedItems = listView.SelectedItems.Cast<DataGridColumn>();
                ViewModel.OnLeftSelectionChanged(selectedItems);
            }
        }

        private void RightListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var selectedItems = listView.SelectedItems.Cast<DataGridColumn>();
                ViewModel.OnRightSelectionChanged(selectedItems);
            }
        }

        private void DPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Search.Focus();
        }
    }
}
