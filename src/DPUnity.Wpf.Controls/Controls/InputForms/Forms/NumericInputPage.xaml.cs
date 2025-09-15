using DPUnity.Windows;
using System.Windows.Input;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    /// <summary>
    /// Interaction logic for NumericInputPage.xaml
    /// </summary>
    public partial class NumericInputPage : DPage
    {
        public NumericInputPage(NumericInputViewModel vm) : base(vm)
        {
            InitializeComponent();
            NumericTextBox.PreviewTextInput += NumericTextBox_PreviewTextInput;
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (DataContext is NumericInputViewModel viewModel)
            {
                e.Handled = !IsValidInput(e.Text, viewModel);
            }
        }

        private bool IsValidInput(string input, NumericInputViewModel viewModel)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            string currentText = NumericTextBox.Text ?? "";
            int caretIndex = NumericTextBox.CaretIndex;

            foreach (char c in input)
            {
                // Cho phép số
                if (char.IsDigit(c))
                    continue;

                // Cho phép dấu âm ở đầu
                if (c == '-' && caretIndex == 0 && !currentText.Contains('-'))
                    continue;

                // Cho phép dấu thập phân nếu AllowDecimal = true và chưa có dấu thập phân
                if (c == '.' && viewModel.AllowDecimal && !currentText.Contains('.'))
                    continue;

                // Ký tự không hợp lệ
                return false;
            }

            // Kiểm tra range validation
            return viewModel.IsValidInputForRange(currentText, input, caretIndex);
        }

        private void DPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            NumericTextBox.Focus();
        }
    }
}
