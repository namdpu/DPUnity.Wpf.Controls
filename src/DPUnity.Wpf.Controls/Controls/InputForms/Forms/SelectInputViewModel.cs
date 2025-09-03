using CommunityToolkit.Mvvm.ComponentModel;
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
        private IInputObject? selectedItem;

        private bool isFirstLoad = true;

        public SelectInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }


        partial void OnSelectedItemChanged(IInputObject? oldValue, IInputObject? newValue)
        {
            if (!isFirstLoad)
            {
                OK();
            }
            isFirstLoad = false;
        }
    }
}
