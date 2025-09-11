using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    /// <summary>
    /// Validation rule for step formula format validation.
    /// </summary>
    public class StepValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets whether empty/null values are allowed. Default is true.
        /// </summary>
        public bool AllowEmpty { get; set; } = true;

        /// <summary>
        /// Gets or sets a custom error message for invalid formula format.
        /// </summary>
        public string? CustomErrorMessage { get; set; }

        private static readonly Regex BracketRegex = new Regex(@"^\[-?\d+(\.\d+)?\]$");
        private static readonly Regex PositiveIntegerRegex = new Regex(@"^[1-9]\d*$");
        private static readonly Regex NumberRegex = new Regex(@"^-?\d+(\.\d+)?$");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? formula = value as string;
            if (string.IsNullOrWhiteSpace(formula))
            {
                if (AllowEmpty)
                {
                    return ValidationResult.ValidResult;
                }
                return new ValidationResult(false, "Công thức không được để trống.");
            }

            string[] rawSteps = formula!.Split(',');
            foreach (string rawStep in rawSteps)
            {
                string trimmed = rawStep.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    return new ValidationResult(false, CustomErrorMessage ?? "Công thức không hợp lệ.");
                }

                if (trimmed.Contains(' '))
                {
                    return new ValidationResult(false, CustomErrorMessage ?? "Công thức không hợp lệ (không được chứa khoảng trắng bên trong step).");
                }

                if (BracketRegex.IsMatch(trimmed))
                {
                    continue; // Valid bracket
                }

                if (trimmed.Contains('*'))
                {
                    string[] parts = trimmed.Split('*');
                    if (parts.Length < 2 || parts.Any(string.IsNullOrEmpty))
                    {
                        return new ValidationResult(false, CustomErrorMessage ?? "Công thức không hợp lệ.");
                    }

                    string firstPart = parts[0];
                    if (!PositiveIntegerRegex.IsMatch(firstPart))
                    {
                        return new ValidationResult(false, CustomErrorMessage ?? "Công thức không hợp lệ.");
                    }

                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (!NumberRegex.IsMatch(parts[i]))
                        {
                            return new ValidationResult(false, CustomErrorMessage ?? "Công thức không hợp lệ.");
                        }
                    }
                }
                else
                {
                    if (!NumberRegex.IsMatch(trimmed))
                    {
                        return new ValidationResult(false, CustomErrorMessage ?? "Công thức không hợp lệ.");
                    }
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}