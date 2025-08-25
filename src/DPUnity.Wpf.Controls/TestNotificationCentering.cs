using System.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService;

namespace DPUnity.Wpf.Controls
{
    /// <summary>
    /// Test class to verify notification centering functionality
    /// </summary>
    public static class TestNotificationCentering
    {
        /// <summary>
        /// Test notification centering with explicit owner window
        /// </summary>
        /// <param name="ownerWindow">The owner window to center the notification on</param>
        public static void TestWithOwner(Window ownerWindow)
        {
            DPDialog.Info("This notification should be centered on the owner window", ownerWindow, "Test Centering");
        }

        /// <summary>
        /// Test notification centering without explicit owner (should find active window)
        /// </summary>
        public static void TestWithoutOwner()
        {
            DPDialog.Info("This notification should be centered on the active window", null, "Test Auto-Centering");
        }

        /// <summary>
        /// Test Ask dialog centering
        /// </summary>
        /// <param name="ownerWindow">The owner window to center the notification on</param>
        /// <returns>User's response</returns>
        public static bool? TestAskDialog(Window ownerWindow)
        {
            return DPDialog.Ask("Do you want to test the centering of this Ask dialog?", ownerWindow, "Test Ask Centering");
        }

        /// <summary>
        /// Test all notification types centering
        /// </summary>
        /// <param name="ownerWindow">The owner window to center the notifications on</param>
        public static void TestAllTypes(Window ownerWindow)
        {
            DPDialog.Info("Information message centered test", ownerWindow, "Info Test");
            DPDialog.Success("Success message centered test", ownerWindow, "Success Test");
            DPDialog.Warning("Warning message centered test", ownerWindow, "Warning Test");
            DPDialog.Error("Error message centered test", ownerWindow, "Error Test");
            
            var result = DPDialog.Ask("Ask dialog centered test - Do you see all dialogs centered?", ownerWindow, "Ask Test");
            DPDialog.Info($"Ask result: {result}", ownerWindow, "Result");
        }
    }
}
