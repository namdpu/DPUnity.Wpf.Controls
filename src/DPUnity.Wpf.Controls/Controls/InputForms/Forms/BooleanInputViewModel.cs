using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using System.Windows.Input;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class BooleanInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private bool value = true;

        [ObservableProperty]
        private string trueContent = "True";

        [ObservableProperty]
        private string falseContent = "False";

        public BooleanInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        [RelayCommand]
        private void Submit()
        {
            OK();
        }

        [RelayCommand]
        private void KeyDown(EventArgs e)
        {
            if (e is not KeyEventArgs keyEventArgs) return;
            switch (keyEventArgs.Key)
            {
                case Key.Space:
                case Key.Enter:
                    // Enter key submits the form
                    OK();
                    keyEventArgs.Handled = true;
                    break;
                case Key.Escape:
                    // Escape key cancels the form
                    Cancel();
                    keyEventArgs.Handled = true;
                    break;
            }
        }
    }
}
