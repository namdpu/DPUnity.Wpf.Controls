using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class NumericValidateRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }

            if (!double.TryParse(value.ToString(), out _))
            {
                return new ValidationResult(false, "Giá trị phải là số.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
