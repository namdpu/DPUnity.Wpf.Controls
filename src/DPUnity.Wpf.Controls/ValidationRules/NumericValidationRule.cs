using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    /// <summary>
    /// Comprehensive numeric validation rule with advanced configuration options.
    /// This is an alias for NumericValidateRule with additional validation features.
    /// </summary>
    public class NumericValidationRule : ValidationRule
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
        /// Gets or sets the minimum value allowed. If null, no minimum limit is applied.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value allowed. If null, no maximum limit is applied.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets whether empty/null values are allowed. Default is false.
        /// </summary>
        public bool AllowEmpty { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of decimal places allowed. If null, no limit is applied.
        /// </summary>
        public int? MaxDecimalPlaces { get; set; }

        /// <summary>
        /// Gets or sets a custom error message for invalid input.
        /// </summary>
        public string? CustomErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the binding path for error context.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property name to extract numeric value from complex objects.
        /// If set, the rule will try to get this property from the bound object.
        /// </summary>
        public string? PropertyPath { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Handle null values
            if (value == null)
            {
                if (AllowEmpty)
                {
                    return ValidationResult.ValidResult;
                }
                return new ValidationResult(false, CustomErrorMessage ?? "Giá trị không được để trống.");
            }

            // If PropertyPath is specified, try to extract the property value
            object actualValue = value;
            if (!string.IsNullOrEmpty(PropertyPath))
            {
                try
                {
                    var property = value.GetType().GetProperty(PropertyPath);
                    if (property != null)
                    {
                        actualValue = property.GetValue(value) ?? string.Empty;
                    }
                    else
                    {
                        return new ValidationResult(false, CustomErrorMessage ?? $"Không tìm thấy thuộc tính '{PropertyPath}'.");
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult(false, CustomErrorMessage ?? $"Lỗi khi truy cập thuộc tính '{PropertyPath}'.");
                }
            }

            // Handle empty string values
            if (actualValue is string strValue && string.IsNullOrWhiteSpace(strValue))
            {
                if (AllowEmpty)
                {
                    return ValidationResult.ValidResult;
                }
                return new ValidationResult(false, CustomErrorMessage ?? "Giá trị không được để trống.");
            }

            string input = actualValue.ToString()!.Trim();

            // Special handling for partial input cases during typing
            string decimalSeparator = cultureInfo?.NumberFormat.NumberDecimalSeparator ?? ".";

            // Allow single minus sign if negative numbers are allowed (user is typing)
            if (input == "-" && AllowNegative)
            {
                return ValidationResult.ValidResult; // Allow partial negative input
            }

            // Allow single decimal separator if decimals are allowed (user is typing)
            if (input == decimalSeparator && AllowDecimal)
            {
                return ValidationResult.ValidResult; // Allow partial decimal input
            }

            // Allow minus followed by decimal separator
            if (input == "-" + decimalSeparator && AllowNegative && AllowDecimal)
            {
                return ValidationResult.ValidResult; // Allow partial negative decimal input
            }

            // Allow partial decimal numbers ending with decimal separator (e.g., "123.")
            if (AllowDecimal && input.EndsWith(decimalSeparator) && input.Length > 1)
            {
                string withoutTrailingDecimal = input.Substring(0, input.Length - decimalSeparator.Length);
                if (double.TryParse(withoutTrailingDecimal, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, cultureInfo ?? CultureInfo.CurrentCulture, out _))
                {
                    return ValidationResult.ValidResult; // Allow partial decimal like "123."
                }
            }

            // Allow negative partial decimal numbers ending with decimal separator (e.g., "-123.")
            if (AllowNegative && AllowDecimal && input.StartsWith("-") && input.EndsWith(decimalSeparator) && input.Length > 2)
            {
                string withoutTrailingDecimal = input.Substring(0, input.Length - decimalSeparator.Length);
                if (double.TryParse(withoutTrailingDecimal, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, cultureInfo ?? CultureInfo.CurrentCulture, out _))
                {
                    return ValidationResult.ValidResult; // Allow partial negative decimal like "-123."
                }
            }

            // Try to parse as double for most comprehensive numeric validation
            NumberStyles numberStyles = NumberStyles.None;

            if (AllowDecimal)
            {
                numberStyles |= NumberStyles.AllowDecimalPoint;
            }

            if (AllowNegative)
            {
                numberStyles |= NumberStyles.AllowLeadingSign;
            }

            numberStyles |= NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

            if (!double.TryParse(input, numberStyles, cultureInfo ?? CultureInfo.CurrentCulture, out double numericValue))
            {
                // Check if it's a negative sign issue
                if (!AllowNegative && input.StartsWith("-"))
                {
                    return new ValidationResult(false, CustomErrorMessage ?? "Không được nhập số âm.");
                }

                // Check if it's a decimal issue
                if (!AllowDecimal && input.Contains(decimalSeparator))
                {
                    return new ValidationResult(false, CustomErrorMessage ?? "Không được nhập số thập phân.");
                }

                return new ValidationResult(false, CustomErrorMessage ?? "Giá trị phải là số hợp lệ.");
            }

            // Check if integer only is required
            if (numericValue != Math.Floor(numericValue))
            {
                return new ValidationResult(false, CustomErrorMessage ?? "Chỉ được nhập số nguyên.");
            }

            // Check decimal places limit (only if decimals are allowed)
            if (MaxDecimalPlaces.HasValue && AllowDecimal && input.Contains(decimalSeparator))
            {
                string[] parts = input.Split(new string[] { decimalSeparator }, StringSplitOptions.None);
                if (parts.Length > 1 && parts[1].Length > MaxDecimalPlaces.Value)
                {
                    return new ValidationResult(false, CustomErrorMessage ?? $"Số thập phân không được vượt quá {MaxDecimalPlaces.Value} chữ số.");
                }
            }

            // Check if negative is allowed
            if (!AllowNegative && numericValue < 0)
            {
                return new ValidationResult(false, CustomErrorMessage ?? "Không được nhập số âm.");
            }

            // Check minimum value
            if (MinValue.HasValue && numericValue < MinValue.Value)
            {
                return new ValidationResult(false, CustomErrorMessage ?? $"Giá trị phải lớn hơn hoặc bằng {MinValue.Value}.");
            }

            // Check maximum value
            if (MaxValue.HasValue && numericValue > MaxValue.Value)
            {
                return new ValidationResult(false, CustomErrorMessage ?? $"Giá trị phải nhỏ hơn hoặc bằng {MaxValue.Value}.");
            }

            // Check range if both min and max are specified
            if (MinValue.HasValue && MaxValue.HasValue && CustomErrorMessage == null)
            {
                if (numericValue < MinValue.Value || numericValue > MaxValue.Value)
                {
                    return new ValidationResult(false, $"Giá trị phải nằm trong khoảng {MinValue.Value} đến {MaxValue.Value}.");
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
