using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class SelectInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private List<IInputObject> itemsSource = [];
        [ObservableProperty]
        private int selectedIndex;
        [ObservableProperty]
        private IInputObject? selectedItem;

        public SelectInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            if (SelectedIndex >= 0)
            {
                OK();
            }
        }

        private bool CanSubmit()
        {
            return SelectedIndex >= 0;
        }

        partial void OnSelectedIndexChanged(int value)
        {
            SubmitCommand.NotifyCanExecuteChanged();
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