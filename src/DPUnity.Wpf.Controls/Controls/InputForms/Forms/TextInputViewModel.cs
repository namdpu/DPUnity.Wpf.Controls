using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Wpf.Common.Windows;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class TextInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string text = string.Empty;

        [ObservableProperty]
        private string inputTitle = string.Empty;


        public TextInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {

        }

        [RelayCommand]
        private void Submit()
        {
            OK();
        }

        [RelayCommand]
        private new void Cancel()
        {
            base.Cancel();
        }

        [RelayCommand]
        private void KeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
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
