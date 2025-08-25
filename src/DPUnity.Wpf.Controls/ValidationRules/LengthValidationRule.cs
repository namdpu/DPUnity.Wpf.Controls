using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class LengthValidationRule : ValidationRule
    {
        public int MinLength { get; set; } = 0;
        public int MaxLength { get; set; } = int.MaxValue;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Giá trị không được để trống.");
            }
            string input = value.ToString() ?? string.Empty;
            int length = input.Length;
            if (length < MinLength || length > MaxLength)
            {
                if (MaxLength == 0)
                {
                    return new ValidationResult(false, $"Độ dài tối thiểu là {MinLength} ký tự.");
                }
                return new ValidationResult(false, $"Độ dài phải từ {MinLength} đến {MaxLength} ký tự.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
