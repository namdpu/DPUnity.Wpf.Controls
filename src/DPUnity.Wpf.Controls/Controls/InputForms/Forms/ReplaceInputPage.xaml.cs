using DPUnity.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for ReplaceInputPage.xaml
    /// </summary>
    public partial class ReplaceInputPage : DPage
    {
        public ReplaceInputPage(ReplaceInputViewModel vm) : base(vm)
        {
            InitializeComponent();
        }

        private void DPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ReplaceTextBox.Focus();
        }
    }
}
