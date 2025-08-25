using System.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService;

namespace DPUnity.Wpf.Controls
{
    /// <summary>
    /// Test window for notification centering functionality
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void BtnTestInfo_Click(object sender, RoutedEventArgs e)
        {
            DPDialog.Info("This is an information dialog that should be centered on this window.", this, "Information Test");
        }

        private void BtnTestSuccess_Click(object sender, RoutedEventArgs e)
        {
            DPDialog.Success("This is a success dialog that should be centered on this window.", this, "Success Test");
        }

        private void BtnTestWarning_Click(object sender, RoutedEventArgs e)
        {
            DPDialog.Warning("This is a warning dialog that should be centered on this window.", this, "Warning Test");
        }

        private void BtnTestError_Click(object sender, RoutedEventArgs e)
        {
            DPDialog.Error("This is an error dialog that should be centered on this window.", this, "Error Test");
        }

        private void BtnTestAsk_Click(object sender, RoutedEventArgs e)
        {
            var result = DPDialog.Ask("This is an ask dialog that should be centered on this window. Do you agree?", this, "Ask Test");
            DPDialog.Info($"You answered: {(result.HasValue ? (result.Value ? "Yes" : "No") : "Canceled")}", this, "Ask Result");
        }

        private void BtnTestAll_Click(object sender, RoutedEventArgs e)
        {
            TestNotificationCentering.TestAllTypes(this);
        }

        private void BtnTestWithoutOwner_Click(object sender, RoutedEventArgs e)
        {
            TestNotificationCentering.TestWithoutOwner();
        }
    }
}
