using System.Windows;

namespace DPUnity.Wpf.Controls.Controls.DialogService
{
    public interface IDPDialog
    {
        bool? Ask(string message, Window? owner = null, string? title = null);
        bool? Error(string message, Window? owner = null, string? title = null);
        bool? Info(string message, Window? owner = null, string? title = null);
        bool? Success(string message, Window? owner = null, string? title = null);
        bool? Warning(string message, Window? owner = null, string? title = null);
        Task<bool?> WeakAsk(string message);
        Task<bool?> WeakError(string message);
        Task<bool?> WeakInfo(string message);
        Task<bool?> WeakSuccess(string message);
        Task<bool?> WeakWarning(string message);
    }
}