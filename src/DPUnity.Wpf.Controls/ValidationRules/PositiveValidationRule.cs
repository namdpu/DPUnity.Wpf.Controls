using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class PositiveValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }

            if (!double.TryParse(value.ToString(), out double number))
            {
                return new ValidationResult(false, "Giá trị phải là số.");
            }

            if (number <= 0)
            {
                return new ValidationResult(false, "Giá trị phải là số dương.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
