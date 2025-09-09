using System.Globalization;
using System.Windows.Data;

namespace DPUnity.Wpf.Controls.Converters
{
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double d ? d.ToString(culture) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && double.TryParse(s, NumberStyles.Any, culture, out double result))
            {
                return result;
            }
            return Binding.DoNothing;
        }
    }
}