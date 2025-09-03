using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class BooleanInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private bool value;

        [ObservableProperty]
        private string trueContent = "True";

        [ObservableProperty]
        private string falseContent = "False";

        public BooleanInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {

        }

        [RelayCommand]
        public void Submit()
        {
            OK();
        }
    }
}
