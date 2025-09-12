using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class RangeFormatValidationRule : ValidationRule
    {
        public bool AllowEmpty { get; set; } = true;
        public string? CustomErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? input = value as string;

            if (string.IsNullOrEmpty(input))
            {
                return AllowEmpty ? ValidationResult.ValidResult : new ValidationResult(false, CustomErrorMessage ?? "Giá trị không được để trống.");
            }

            string invalidFormatMessage = CustomErrorMessage ?? "Định dạng không hợp lệ.";

            if (!input!.StartsWith("["))
            {
                return new ValidationResult(false, invalidFormatMessage);
            }

            int closeIdx = input.IndexOf(']', 1);
            if (closeIdx == -1)
            {
                return new ValidationResult(false, invalidFormatMessage);
            }

            string firstPart = input.Substring(1, closeIdx - 1);
            string rest = input.Substring(closeIdx + 1);

            if (string.IsNullOrEmpty(firstPart))
            {
                return new ValidationResult(false, invalidFormatMessage);
            }

            if (firstPart.Contains(".."))
            {
                // Định dạng 2: [a..b]
                if (!string.IsNullOrEmpty(rest))
                {
                    return new ValidationResult(false, invalidFormatMessage);
                }

                string[] parts = firstPart.Split([".."], StringSplitOptions.None);
                if (parts.Length != 2)
                {
                    return new ValidationResult(false, invalidFormatMessage);
                }

                if (!double.TryParse(parts[0], NumberStyles.Any, cultureInfo, out double a))
                {
                    return new ValidationResult(false, invalidFormatMessage);
                }

                if (!double.TryParse(parts[1], NumberStyles.Any, cultureInfo, out double b))
                {
                    return new ValidationResult(false, invalidFormatMessage);
                }

                if (b <= a)
                {
                    return new ValidationResult(false, CustomErrorMessage ?? "Điểm kết thúc phải lớn hơn điểm bắt đầu.");
                }

                return ValidationResult.ValidResult;
            }
            else
            {
                // Định dạng 1: [a]len hoặc Định dạng 3: [a]..[b]
                if (!double.TryParse(firstPart, NumberStyles.Any, cultureInfo, out double a))
                {
                    return new ValidationResult(false, invalidFormatMessage);
                }

                if (string.IsNullOrEmpty(rest))
                {
                    return new ValidationResult(false, invalidFormatMessage);
                }

                if (rest.StartsWith(".."))
                {
                    // Định dạng 3: [a]..[b]
                    string bPart = rest.Substring(2);
                    if (!bPart.StartsWith("[") || !bPart.EndsWith("]"))
                    {
                        return new ValidationResult(false, invalidFormatMessage);
                    }

                    string bStr = bPart.Substring(1, bPart.Length - 2);
                    if (!double.TryParse(bStr, NumberStyles.Any, cultureInfo, out double b))
                    {
                        return new ValidationResult(false, invalidFormatMessage);
                    }

                    return ValidationResult.ValidResult;
                }
                else
                {
                    // Định dạng 1: [a]len
                    if (!double.TryParse(rest, NumberStyles.Any, cultureInfo, out double len))
                    {
                        return new ValidationResult(false, invalidFormatMessage);
                    }

                    if (len <= 0)
                    {
                        return new ValidationResult(false, CustomErrorMessage ?? "Độ dài phải lớn hơn 0.");
                    }

                    return ValidationResult.ValidResult;
                }
            }
        }
    }
}