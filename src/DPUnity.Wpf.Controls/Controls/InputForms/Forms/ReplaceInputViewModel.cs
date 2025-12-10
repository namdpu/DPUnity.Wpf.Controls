using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Wpf.Common.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class ReplaceInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string replace = string.Empty;

        [ObservableProperty]
        private string replaceWith = string.Empty;

        [ObservableProperty]
        private string inputTitle = string.Empty;

        public ReplaceInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        partial void OnReplaceChanged(string value)
        {
            SubmitCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            OK();
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrEmpty(Replace);
        }

        [RelayCommand]
        private new void Cancel()
        {
            base.Cancel();
        }

        [RelayCommand]
        private void KeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && CanSubmit())
            {
                Submit();
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                Cancel();
                e.Handled = true;
            }
        }
    }
}