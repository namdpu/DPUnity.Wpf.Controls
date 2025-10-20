using System.Globalization;
using System.Windows.Data;

namespace DPUnity.Wpf.Controls.Converters
{
    /// <summary>
    /// Converter to convert DateTimeOffset or DateTime from UTC to local DateTime
    /// </summary>
    public class APIDateTimeToLocalDateTimeConverter : IValueConverter
    {
        private static APIDateTimeToLocalDateTimeConverter? _instance;
        public static APIDateTimeToLocalDateTimeConverter Instance => _instance ??= new APIDateTimeToLocalDateTimeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Xử lý trường hợp value là DateTime
                if (value is DateTime dateTime)
                {
                    // Giả định API trả về UTC, chuyển sang local time
                    if (dateTime.Kind == DateTimeKind.Unspecified)
                    {
                        // Nếu không xác định Kind, giả định là UTC
                        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
                    }
                    else if (dateTime.Kind == DateTimeKind.Utc)
                    {
                        // Nếu đã là UTC, chuyển trực tiếp sang local
                        return dateTime.ToLocalTime();
                    }
                    // Nếu đã là local hoặc Kind khác, trả về nguyên gốc
                    return dateTime;
                }
                // Xử lý trường hợp value là DateTimeOffset
                else if (value is DateTimeOffset dateTimeOffset)
                {
                    return dateTimeOffset.LocalDateTime;
                }

                // Trả về giá trị gốc nếu không phải các kiểu trên
                return value;
            }
            catch (Exception)
            {
                // Trong trường hợp lỗi, trả về giá trị gốc
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Xử lý chuyển đổi ngược từ local về UTC
                if (value is DateTime dateTime)
                {
                    if (dateTime.Kind == DateTimeKind.Unspecified)
                    {
                        // Giả định là local time, chuyển về UTC
                        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
                    }
                    else if (dateTime.Kind == DateTimeKind.Local)
                    {
                        // Nếu đã là local, chuyển trực tiếp về UTC
                        return dateTime.ToUniversalTime();
                    }
                    return dateTime;
                }
                else if (value is DateTimeOffset dateTimeOffset)
                {
                    return dateTimeOffset.UtcDateTime;
                }

                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}