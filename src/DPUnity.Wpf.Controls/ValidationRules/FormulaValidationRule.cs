using System.Globalization;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    public class FormulaValidationRule : ValidationRule
    {
        public bool AllowEmpty { get; set; } = true;
        public bool AllowParameter { get; set; } = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string formula = value?.ToString()?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(formula))
            {
                return AllowEmpty ? ValidationResult.ValidResult : new ValidationResult(false, "Công thức không được để trống.");
            }

            if (IsValidFormula(formula, AllowParameter))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Công thức không hợp lệ.");
        }

        private bool IsValidFormula(string formula, bool allowParam)
        {
            formula = new string([.. formula.Where(c => !char.IsWhiteSpace(c))]); // Loại bỏ khoảng trắng
            int pos = 0;
            return ParseExpression(formula, ref pos, allowParam) && pos == formula.Length;
        }

        private bool ParseExpression(string s, ref int pos, bool allowParam)
        {
            if (!ParseTerm(s, ref pos, allowParam)) return false;
            while (pos < s.Length && (s[pos] == '+' || s[pos] == '-'))
            {
                pos++;
                if (!ParseTerm(s, ref pos, allowParam)) return false;
            }
            return true;
        }

        private bool ParseTerm(string s, ref int pos, bool allowParam)
        {
            if (!ParseFactor(s, ref pos, allowParam)) return false;
            while (pos < s.Length && (s[pos] == '*' || s[pos] == '/'))
            {
                pos++;
                if (!ParseFactor(s, ref pos, allowParam)) return false;
            }
            return true;
        }

        private bool ParseFactor(string s, ref int pos, bool allowParam)
        {
            if (pos >= s.Length) return false;
            char c = s[pos];

            if (c == '-')
            {
                pos++;
                return ParseFactor(s, ref pos, allowParam);
            }
            else if (char.IsDigit(c) || c == '.')
            {
                return FormulaValidationRule.ParseNumber(s, ref pos);
            }
            else if (c == '(')
            {
                pos++;
                if (!ParseExpression(s, ref pos, allowParam)) return false;
                if (pos >= s.Length || s[pos] != ')') return false;
                pos++;
                return true;
            }
            else if (c == '[' && allowParam)
            {
                return ParseParameter(s, ref pos);
            }

            return false;
        }

        private static bool ParseNumber(string s, ref int pos)
        {
            bool hasDot = false;
            bool hasDigit = false;
            while (pos < s.Length)
            {
                char c = s[pos];
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    pos++;
                }
                else if (c == '.' && !hasDot)
                {
                    hasDot = true;
                    pos++;
                }
                else
                {
                    break;
                }
            }
            return hasDigit;
        }

        private bool ParseParameter(string s, ref int pos)
        {
            if (pos >= s.Length || s[pos] != '[') return false;
            pos++;
            if (!ParseName(s, ref pos)) return false;
            if (pos < s.Length && s[pos] == '~')
            {
                pos++;
                if (!ParseName(s, ref pos)) return false;
            }
            if (pos >= s.Length || s[pos] != ']') return false;
            pos++;
            return true;
        }

        private bool ParseName(string s, ref int pos)
        {
            bool hasChar = false;
            while (pos < s.Length && (char.IsLetterOrDigit(s[pos]) || s[pos] == '_'))
            {
                hasChar = true;
                pos++;
            }
            return hasChar;
        }
    }
}