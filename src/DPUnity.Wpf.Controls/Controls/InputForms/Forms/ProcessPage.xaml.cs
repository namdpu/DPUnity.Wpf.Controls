using DPUnity.Windows;
using DPUnity.Wpf.Common.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for ProcessPage.xaml
    /// </summary>
    public partial class ProcessPage : BaseProcessPage
    {
        public ProcessPage(IProcessViewModel processViewModel) : base(processViewModel)
        {
            InitializeComponent();
            CancelButton.Focus();
        }
    }
}