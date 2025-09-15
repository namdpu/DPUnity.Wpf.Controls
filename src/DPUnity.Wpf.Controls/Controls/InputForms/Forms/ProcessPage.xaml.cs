using DPUnity.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for ProcessPage.xaml
    /// </summary>
    public partial class ProcessPage : Windows.ProcessPage // Changed base class to match the other partial declaration
    {
        public ProcessPage(ProcessViewModel processViewModel) : base(processViewModel)
        {
            InitializeComponent();
            CancelButton.Focus();
        }
    }
}