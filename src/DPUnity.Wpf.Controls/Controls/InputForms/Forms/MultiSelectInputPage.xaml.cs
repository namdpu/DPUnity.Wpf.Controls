using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for MultiSelectInputPage.xaml
    /// </summary>
    public partial class MultiSelectInputPage : DPage
    {
        private new MultiSelectInputViewModel ViewModel => (MultiSelectInputViewModel)DataContext;

        public MultiSelectInputPage(MultiSelectInputViewModel vm) : base(vm)
        {
            InitializeComponent();
        }

        private void LeftListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var selectedItems = listView.SelectedItems.Cast<IInputObject>();
                ViewModel.OnLeftSelectionChanged(selectedItems);
            }
        }

        private void RightListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var selectedItems = listView.SelectedItems.Cast<IInputObject>();
                ViewModel.OnRightSelectionChanged(selectedItems);
            } 
        }
    }
}
