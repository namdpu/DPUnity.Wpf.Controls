using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;

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
    }
}
