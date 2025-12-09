using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using System.Windows.Input;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class ConfirmDeleteViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string message = "Are you sure you want to delete this item?";

        [ObservableProperty]
        private string inputString = string.Empty;

        public string ConfirmString { get; set; } = "DELETE";

        public ConfirmDeleteViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
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
                    if (!CanSubmit) break;
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

        public bool CanSubmit => InputString == ConfirmString;

        partial void OnInputStringChanged(string? oldValue, string newValue)
        {
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }
}
