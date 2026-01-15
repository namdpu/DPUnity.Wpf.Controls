using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Wpf.Common.Windows;
using System.Globalization;
using System.Windows.Input;

namespace DPUnity.Wpf.Controls.Controls.InputForms.Forms
{
    public partial class NumericInputViewModel : ViewModelPage
    {
        [ObservableProperty]
        private string text = string.Empty;

        [ObservableProperty]
        private string inputTitle = string.Empty;

        [ObservableProperty]
        private bool allowDecimal = true;

        [ObservableProperty]
        private double? numericValue;

        [ObservableProperty]
        private double? minValue;

        [ObservableProperty]
        private double? maxValue;

        [ObservableProperty]
        private string rangeDisplayText = string.Empty;

        [ObservableProperty]
        private bool hasValidationRange = false;

        [ObservableProperty]
        private bool allowEmpty = true;

        public NumericInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                NumericValue = null;
                OK();
            }
            else if (ValidateInput())
            {
                OK();
            }
        }

        private bool CanSubmit()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return AllowEmpty;
            }
            else
            {
                return ValidateInput();
            }
        }

        [RelayCommand]
        private new void Cancel()
        {
            base.Cancel();
        }

        private bool ValidateInput()
        {
            if (!double.TryParse(Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out double value))
                return false;

            // Kiểm tra nếu chỉ cho phép số nguyên
            if (!AllowDecimal && value != Math.Truncate(value))
                return false;

            // Kiểm tra giá trị min/max
            if (MinValue.HasValue && value < MinValue.Value)
                return false;

            if (MaxValue.HasValue && value > MaxValue.Value)
                return false;

            NumericValue = value;
            return true;
        }

        partial void OnTextChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out double numericVal))
            {
                NumericValue = numericVal;
            }
            else
            {
                NumericValue = null;
            }

            SubmitCommand.NotifyCanExecuteChanged();
        }

        partial void OnAllowDecimalChanged(bool value)
        {
            if (!value && !string.IsNullOrEmpty(Text) && Text.Contains('.'))
            {
                if (double.TryParse(Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out double decimalValue))
                {
                    Text = Math.Truncate(decimalValue).ToString(CultureInfo.InvariantCulture);
                }
            }

            SubmitCommand.NotifyCanExecuteChanged();
        }

        partial void OnMinValueChanged(double? value)
        {
            UpdateRangeDisplay();
            SubmitCommand.NotifyCanExecuteChanged();
        }

        partial void OnMaxValueChanged(double? value)
        {
            UpdateRangeDisplay();
            SubmitCommand.NotifyCanExecuteChanged();
        }

        private void UpdateRangeDisplay()
        {
            if (MinValue.HasValue && MaxValue.HasValue)
            {
                RangeDisplayText = $"[{MinValue.Value} : {MaxValue.Value}]";
                HasValidationRange = true;
            }
            else if (MinValue.HasValue)
            {
                RangeDisplayText = $"Min: {MinValue.Value}";
                HasValidationRange = true;
            }
            else if (MaxValue.HasValue)
            {
                RangeDisplayText = $"Max: {MaxValue.Value}";
                HasValidationRange = true;
            }
            else
            {
                RangeDisplayText = string.Empty;
                HasValidationRange = false;
            }
        }

        /// <summary>
        /// Kiểm tra xem việc nhập ký tự mới có tạo ra giá trị hợp lệ hay không
        /// Cho phép các trạng thái trung gian khi nhập (ví dụ: "0" khi muốn nhập "0.5")
        /// Validation cuối cùng sẽ được thực hiện bởi CanSubmit/ValidateInput
        /// </summary>
        public bool IsValidInputForRange(string currentText, string newInput, int caretIndex)
        {
            if (string.IsNullOrEmpty(newInput))
                return false;

            // Tạo text giả định sau khi nhập
            string projectedText = currentText.Insert(caretIndex, newInput);

            // Nếu text rỗng hoặc chỉ có dấu âm thì cho phép
            if (string.IsNullOrEmpty(projectedText) || projectedText == "-")
                return true;

            // Kiểm tra xem có parse được thành số không
            if (!double.TryParse(projectedText, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out double value))
                return true; // Nếu không parse được thì để cho logic khác xử lý

            // Cho phép nhập các giá trị trung gian (không kiểm tra range ở đây)
            // Range sẽ được kiểm tra trong ValidateInput khi submit
            // Điều này cho phép người dùng nhập "0" khi muốn nhập "0.5" (ngay cả khi min = 0.01)
            return true;
        }

        [RelayCommand]
        private void KeyDown(EventArgs e)
        {
            if (e is KeyEventArgs keyEvent)
            {
                if (keyEvent.Key == Key.Enter && CanSubmit())
                {
                    Submit();
                    keyEvent.Handled = true;
                }
                else if (keyEvent.Key == Key.Escape)
                {
                    Cancel();
                    keyEvent.Handled = true;
                }
            }
        }
    }
}