using System.Globalization;
using System.Reflection;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.ValidationRules
{
    /// <summary>
    /// Validation rule for checking string length constraints with support for complex object property validation.
    /// </summary>
    public class LengthValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets the minimum length allowed. Default is 0.
        /// </summary>
        public int MinLength { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum length allowed. Default is int.MaxValue (unlimited).
        /// </summary>
        public int MaxLength { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets whether empty/null values are allowed. Default is false.
        /// </summary>
        public bool AllowEmpty { get; set; } = false;

        /// <summary>
        /// Gets or sets a custom error message for invalid input.
        /// </summary>
        public string? CustomErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the property path to extract value from complex objects.
        /// If set, the rule will try to get this property from the bound object.
        /// Example: "Name" will get obj.Name, "User.Name" will get obj.User.Name
        /// </summary>
        public string? PropertyPath { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Xử lý giá trị null
            if (value == null)
            {
                return new ValidationResult(false, CustomErrorMessage ?? "Giá trị không được để trống.");
            }

            // Nếu PropertyPath được thiết lập, cố gắng lấy thuộc tính từ object
            object actualValue = value;
            if (!string.IsNullOrEmpty(PropertyPath))
            {
                try
                {
                    var property = GetPropertyValue(value, PropertyPath!);
                    if (property != null)
                    {
                        actualValue = property;
                    }
                    else
                    {
                        actualValue = string.Empty;
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult(false, CustomErrorMessage ?? $"Không thể truy cập thuộc tính '{PropertyPath}'.");
                }
            }

            string input = actualValue?.ToString() ?? string.Empty;
            int length = input.Length;

            // Xử lý chuỗi rỗng
            if (length == 0)
            {
                if (MinLength > 0)
                {
                    return new ValidationResult(false, CustomErrorMessage ?? $"Độ dài tối thiểu là {MinLength} ký tự.");
                }
                return ValidationResult.ValidResult; // Nếu MinLength = 0 và không AllowEmpty thì chuỗi rỗng vẫn ok
            }

            // Kiểm tra độ dài tối thiểu
            if (length < MinLength)
            {
                return new ValidationResult(false, CustomErrorMessage ?? $"Độ dài tối thiểu là {MinLength} ký tự.");
            }

            // Kiểm tra độ dài tối đa (chỉ khi có giới hạn)
            if (MaxLength != int.MaxValue && length > MaxLength)
            {
                if (MinLength > 0)
                {
                    return new ValidationResult(false, CustomErrorMessage ?? $"Độ dài phải từ {MinLength} đến {MaxLength} ký tự.");
                }
                else
                {
                    return new ValidationResult(false, CustomErrorMessage ?? $"Độ dài tối đa là {MaxLength} ký tự.");
                }
            }

            return ValidationResult.ValidResult;
        }

        /// <summary>
        /// Gets property value from object using reflection.
        /// Supports nested properties (e.g., "User.Name")
        /// </summary>
        /// <param name="obj">Object containing the property</param>
        /// <param name="propertyPath">Property path</param>
        /// <returns>Property value or null if not found</returns>
        private object? GetPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
                return null;

            string[] properties = propertyPath.Split('.');
            object? currentObject = obj;

            foreach (string property in properties)
            {
                if (currentObject == null)
                    return null;

                PropertyInfo? propertyInfo = currentObject.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    return null;

                currentObject = propertyInfo.GetValue(currentObject);
            }

            return currentObject;
        }
    }
}
