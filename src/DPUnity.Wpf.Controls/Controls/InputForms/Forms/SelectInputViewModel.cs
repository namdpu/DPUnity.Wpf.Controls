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

        public SelectInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        partial void OnSelectedItemChanged(IInputObject? value)
        {
            base.OK();
        }
    }
}
