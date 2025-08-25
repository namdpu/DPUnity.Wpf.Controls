using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DPUnity.Wpf.Controls.Controls.ColorPickers
{
    /// <summary>
    /// Static manager class để lưu trữ và quản lý các màu đã pick gần đây
    /// </summary>
    public static class RecentColorsManager
    {
        private static readonly List<Color> _recentColors = new List<Color>();
        private static readonly object _lockObject = new object();
        
        /// <summary>
        /// Số lượng màu tối đa được lưu trong danh sách recent colors
        /// </summary>
        public const int MaxRecentColors = 12;

        /// <summary>
        /// Danh sách các màu đã pick gần đây (read-only)
        /// </summary>
        public static IReadOnlyList<Color> RecentColors
        {
            get
            {
                lock (_lockObject)
                {
                    return _recentColors.ToList();
                }
            }
        }

        /// <summary>
        /// Event được fire khi danh sách recent colors thay đổi
        /// </summary>
        public static event System.EventHandler? RecentColorsChanged;

        /// <summary>
        /// Thêm một màu vào danh sách recent colors
        /// </summary>
        /// <param name="color">Màu cần thêm</param>
        public static void AddRecentColor(Color color)
        {
            lock (_lockObject)
            {
                // Loại bỏ màu này nếu đã tồn tại trong danh sách
                _recentColors.RemoveAll(c => c.R == color.R && c.G == color.G && c.B == color.B && c.A == color.A);
                
                // Thêm màu vào đầu danh sách
                _recentColors.Insert(0, color);
                
                // Giới hạn số lượng màu trong danh sách
                while (_recentColors.Count > MaxRecentColors)
                {
                    _recentColors.RemoveAt(_recentColors.Count - 1);
                }
            }

            // Fire event thông báo danh sách đã thay đổi
            RecentColorsChanged?.Invoke(null, System.EventArgs.Empty);
        }

        /// <summary>
        /// Xóa tất cả màu trong danh sách recent colors
        /// </summary>
        public static void ClearRecentColors()
        {
            lock (_lockObject)
            {
                _recentColors.Clear();
            }

            // Fire event thông báo danh sách đã thay đổi
            RecentColorsChanged?.Invoke(null, System.EventArgs.Empty);
        }

        /// <summary>
        /// Kiểm tra xem một màu có trong danh sách recent colors không
        /// </summary>
        /// <param name="color">Màu cần kiểm tra</param>
        /// <returns>True nếu màu đã tồn tại trong danh sách</returns>
        public static bool ContainsColor(Color color)
        {
            lock (_lockObject)
            {
                return _recentColors.Any(c => c.R == color.R && c.G == color.G && c.B == color.B && c.A == color.A);
            }
        }
    }
}
