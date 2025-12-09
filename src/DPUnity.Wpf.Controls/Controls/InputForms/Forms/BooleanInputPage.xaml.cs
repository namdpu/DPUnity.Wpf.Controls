using DPUnity.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for BooleanInputPage.xaml
    /// </summary>
    public partial class BooleanInputPage : DPage
    {
        public BooleanInputPage(BooleanInputViewModel vm) : base(vm)
        {
            InitializeComponent();
        }

        private void DPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is BooleanInputViewModel vm)
            {
                vm.Value = true;

                if (TrueRadio != null)
                {
                    TrueRadio.GotFocus += (s, args) => vm.Value = true;
                    TrueRadio.Focus();
                }

                if (FalseRadio != null)
                {
                    FalseRadio.GotFocus += (s, args) => FalseRadio.IsChecked = true;
                }
            }
        }
    }
}