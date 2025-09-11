using System.Globalization;
using System.Windows.Data;

namespace DPUnity.Wpf.Controls.Converters
{
    public sealed class MathConverter : IMultiValueConverter
    {
        public MathOperation Operation { get; set; }

        public object? Convert(object?[]? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is null || value.Length < 2 || value[0] is null || value[1] is null) return Binding.DoNothing;

            if (!double.TryParse(value[0]!.ToString(), out double value1) || !double.TryParse(value[1]!.ToString(), out double value2))
                return 0;

            return Operation switch
            {
                MathOperation.Divide => value1 / value2,
                MathOperation.Multiply => value1 * value2,
                MathOperation.Subtract => value1 - value2,
                MathOperation.Pow => Math.Pow(value1, value2),
                _ => (object)(value1 + value2),
            };
        }

        public object?[]? ConvertBack(object? value, Type[]? targetTypes, object? parameter, CultureInfo? culture)
            => throw new NotImplementedException();
    }
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Pow
    }
}
