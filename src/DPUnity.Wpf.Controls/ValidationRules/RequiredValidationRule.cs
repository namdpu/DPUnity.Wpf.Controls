using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class RequiredValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? input = value as string;
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, "Trường này là bắt buộc.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
