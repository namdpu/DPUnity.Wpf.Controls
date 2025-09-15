using DPUnity.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for TextInputPage.xaml
    /// </summary>
    public partial class TextInputPage : DPage
    {
        public TextInputPage(TextInputViewModel vm) : base(vm)
        {
            InitializeComponent();
        }

        private void DPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InputTextBox.Focus();
        }
    }
}
