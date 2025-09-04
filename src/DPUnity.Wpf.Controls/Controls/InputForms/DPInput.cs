using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService;
using DPUnity.Wpf.Controls.Helpers;
using System.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms
{
    public static class DPInput
    {
        public static ProcessViewModel ShowProcess(string title, nint owner = 0)
        {
            var windowOptions = new WindowOptions()
            {
                ResizeMode = ResizeMode.NoResize,
                MinWidth = 125,
                MinHeight = 125,
                Width = 450,
                Height = 125,
                Title = title,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };

            var viewModel = WindowManager.OpenWindowProcess<Forms.ProcessPage>(() =>
            {
                return DPDialog.Ask($"Bạn có muốn dừng tiến trình đang chạy không?") == true;
            }, windowOptions);
            return viewModel;
        }
    }
}
