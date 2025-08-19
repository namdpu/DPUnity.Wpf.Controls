namespace DPUnity.Wpf.Controls.Interfaces
{
    public class AppHwnd
    {
        private AppHwnd() { }

        public static AppHwnd Instance { get; } = new();

        public IntPtr? Hwnd { internal get; set; }
    }
}