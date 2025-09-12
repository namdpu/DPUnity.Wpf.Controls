using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    /// <summary>
    /// Validation rule for email address format validation using System.Net.Mail.MailAddress.
    /// </summary>
    public class EmailValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets whether empty/null values are allowed. Default is true.
        /// </summary>
        public bool AllowEmpty { get; set; } = true;

        /// <summary>
        /// Gets or sets a custom error message for invalid email format.
        /// </summary>
        public string? CustomErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                if (AllowEmpty)
                {
                    return ValidationResult.ValidResult;
                }
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
