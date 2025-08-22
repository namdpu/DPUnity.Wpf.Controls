using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult(false, "Email không được để trống.");
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    return new ValidationResult(false, "Email không hợp lệ.");
                }
            }
            catch
            {
                return new ValidationResult(false, "Email không hợp lệ.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
