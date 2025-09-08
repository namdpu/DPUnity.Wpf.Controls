using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
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

        public static async Task<InputBooleanResult> ShowBooleanInput(string title, string trueContent = "Đúng", string falseContent = "Sai", bool defaultValue = false, nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 300,
                Height = 160,
                MinHeight = 160,
                ResizeMode = System.Windows.ResizeMode.NoResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<BooleanInputPage, BooleanInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is BooleanInputViewModel viewModel)
                   {
                       viewModel.TrueContent = trueContent;
                       viewModel.FalseContent = falseContent;
                       viewModel.Value = defaultValue;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputBooleanResult(Result, ViewModel.Value);
            }
            return new InputBooleanResult(Result, false);
        }
    }
}
