using DPUnity.Windows;
using System.Windows.Input;
using static DPUnity.Wpf.Controls.Controls.DialogService.DPDialog;

namespace DPUnity.Wpf.Controls.Controls.DialogService.Views
{
    /// <summary>
    /// Interaction logic for NotificationPage.xaml
    /// </summary>
    public partial class NotificationPage : DPage
    {
        public NotificationViewModel NotiViewModel => (NotificationViewModel)ViewModel;
        public NotificationPage(NotificationViewModel vm) : base(vm)
        {
            InitializeComponent();
            Loaded += OnLoaded;
            PreviewKeyDown += OnKeyDown;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NotiViewModel.Type == NotificationType.Ask)
            {
                OKButton.Focus();
                Keyboard.Focus(OKButton);
            }
            else
            {
                CloseButton.Focus();
                Keyboard.Focus(CloseButton);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    HandleEscapeKey();
                    e.Handled = true;
                    break;

                case Key.Enter:
                case Key.Space:
                    HandleEnterOrSpaceKey();
                    e.Handled = true;
                    break;
            }
        }

        private void HandleEscapeKey()
        {
            if (NotiViewModel.Type == NotificationType.Ask)
            {
                // Trường hợp Ask: ESC => Result = null, đóng form
                NotiViewModel.CloseCommand.Execute(null);
            }
            else
            {
                // Trường hợp không phải Ask: ESC => đóng form
                NotiViewModel.CloseCommand.Execute(null);
            }
        }

        private void HandleEnterOrSpaceKey()
        {
            if (NotiViewModel.Type == NotificationType.Ask)
            {
                // Trường hợp Ask: Enter/Space => thực hiện action của nút đang focus
                var focusedElement = Keyboard.FocusedElement as System.Windows.FrameworkElement;

                if (focusedElement == OKButton)
                {
                    NotiViewModel.OkActionCommand.Execute(null);
                }
                else if (focusedElement == CancelButton)
                {
                    NotiViewModel.CancelCommand.Execute(null);
                }
                else
                {
                    // Mặc định thực hiện action của nút Yes (OK)
                    NotiViewModel.OkActionCommand.Execute(null);
                }
            }
            else
            {
                // Trường hợp không phải Ask: Enter/Space => đóng form
                NotiViewModel.CloseCommand.Execute(null);
            }
        }
    }
}
