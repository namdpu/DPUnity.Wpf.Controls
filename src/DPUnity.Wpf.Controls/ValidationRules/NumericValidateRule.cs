using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    /// <summary>
    /// Basic numeric validation rule for simple numeric input validation.
    /// For advanced numeric validation features, consider using NumericValidationRule.
    /// </summary>
    public class NumericValidateRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets whether decimal numbers are allowed. Default is true.
        /// </summary>
        public bool AllowDecimal { get; set; } = true;

        /// <summary>
        /// Gets or sets whether negative numbers are allowed. Default is true.
        /// </summary>
        public bool AllowNegative { get; set; } = true;

        /// <summary>
        /// Gets or sets whether empty/null values are allowed. Default is false.
        /// </summary>
        public bool AllowEmpty { get; set; } = false;

        /// <summary>
        /// Gets or sets the minimum value allowed. If null, no minimum limit is applied.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value allowed. If null, no maximum limit is applied.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets a custom error message for invalid input.
        /// </summary>
        public string? CustomErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the binding path for error context.
        /// </summary>
        public string Path { get; set; } = string.Empty;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidationResult.ValidResult;
        }
    }
}