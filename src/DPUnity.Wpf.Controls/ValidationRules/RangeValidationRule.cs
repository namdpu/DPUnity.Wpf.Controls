using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class RangeValidationRule : ValidationRule
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.ValidResult; // Để required xử lý riêng
            }

            if (!double.TryParse(value.ToString(), out double number))
            {
                return new ValidationResult(false, "Giá trị phải là số.");
            }

            if (number < Min || number > Max)
            {
                return new ValidationResult(false, $"Giá trị phải nằm trong khoảng {Min} đến {Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
