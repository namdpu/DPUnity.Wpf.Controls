using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DPUnity.Windows;
using DPUnity.Windows.Services;
using System.Globalization;

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
        private bool allowEmpty = false;

        public NumericInputViewModel(IWindowService windowService, INavigationService navigationService) : base(windowService, navigationService)
        {
        }

        [RelayCommand]
        private void Submit()
        {
            if (AllowEmpty && string.IsNullOrWhiteSpace(Text))
            {
                NumericValue = null;
                OK();
            }
            else if (ValidateInput())
            {
                OK();
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
        }

        partial void OnMinValueChanged(double? value)
        {
            UpdateRangeDisplay();
        }

        partial void OnMaxValueChanged(double? value)
        {
            UpdateRangeDisplay();
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

            // Kiểm tra range
            if (MinValue.HasValue && value < MinValue.Value)
                return false;

            if (MaxValue.HasValue && value > MaxValue.Value)
                return false;

            return true;
        }
    }
}
