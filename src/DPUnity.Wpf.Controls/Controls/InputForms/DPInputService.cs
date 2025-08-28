using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;

namespace DPUnity.Wpf.Controls.Controls.InputForms
{
    public class DPInputService
    {
        private readonly IWindowService _windowService;
        public DPInputService(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public async Task<InputResult> ShowTextInput(string title, string inputTitle, string defaultText)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 170,
                MinHeight = 170,
                ResizeMode = System.Windows.ResizeMode.NoResize,
            };
            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<TextInputPage, TextInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is TextInputViewModel viewModel)
                   {
                       viewModel.Text = defaultText;
                       viewModel.InputTitle = inputTitle;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputResult(Result, ViewModel.Text);
            }
            return new InputResult(Result, string.Empty);
        }


    }

    public struct InputResult(MessageResult isOK, string text)
    {
        public bool IsOk => isOK == MessageResult.OK;
        public string Text => text;
    }






}
