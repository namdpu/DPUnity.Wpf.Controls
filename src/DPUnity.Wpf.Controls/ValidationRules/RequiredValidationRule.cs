using System.Globalization;
using System.Windows.Controls;
namespace DPUnity.Wpf.Controls.ValidationRules
{
    /// <summary>
    /// Validation rule to ensure a value is required (not null, empty, or default value).
    /// </summary>
    public class RequiredValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets a custom error message for required field validation.
        /// </summary>
        public string? CustomErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the property path to extract value from complex objects.
        /// If set, the rule will try to get this property from the bound object.
        /// </summary>
        public string? PropertyPath { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Trường này là bắt buộc.");
            }
            if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            {
                return new ValidationResult(false, "Trường này là bắt buộc.");
            }
            if (value.GetType().IsValueType && value.Equals(System.Activator.CreateInstance(value.GetType())))
            {
                return new ValidationResult(false, "Trường này là bắt buộc.");
            }
            return ValidationResult.ValidResult; // Object != null là valid nếu không chỉ định path

        }
    }
}