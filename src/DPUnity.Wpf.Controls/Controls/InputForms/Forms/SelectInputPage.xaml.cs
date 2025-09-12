using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for SelectInputPage.xaml
    /// </summary>
    public partial class SelectInputPage : DPage
    {
        public SelectInputPage(SelectInputViewModel vm) : base(vm)
        {
            InitializeComponent();
        }
    }
}
