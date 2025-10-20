using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService;
using DPUnity.Wpf.Controls.Controls.DialogService.Views;
using DPUnity.Wpf.Controls.Controls.InputForms;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
using DPUnity.Wpf.Controls.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DPUnity.Wpf.Controls.DependencyInjections
{
    public static class RegisterServices
    {
        public static IServiceCollection AddDPUControls(this IServiceCollection services)
        {
            services.AddScoped<IDPInputService, DPInputService>();
            services.AddWindowWpf<IDPDialogWindow, DPDialogWindowUI>();
            services.AddScoped<IDPDialogService, DPDialogService>();

            #region Input forms
            services.AddScoped<TextInputPage>();
            services.AddScoped<TextInputViewModel>();

            services.AddScoped<NumericInputPage>();
            services.AddScoped<NumericInputViewModel>();

            services.AddScoped<SelectInputPage>();
            services.AddScoped<SelectInputViewModel>();

            services.AddScoped<MultiSelectInputPage>();
            services.AddScoped<MultiSelectInputViewModel>();

            services.AddScoped<BooleanInputPage>();
            services.AddScoped<BooleanInputViewModel>();

            services.AddScoped<ReplaceInputPage>();
            services.AddScoped<ReplaceInputViewModel>();

            services.AddScoped<DataGridReplaceInputPage>();
            services.AddScoped<DataGridReplaceInputViewModel>();

            services.AddScoped<Controls.InputForms.Forms.ProcessPage>();

            services.AddScoped<ConfirmDeletePage>();
            services.AddScoped<ConfirmDeleteViewModel>();
            #endregion

            #region DialogServices
            services.AddScoped<NotificationPage>();
            services.AddScoped<NotificationViewModel>();
            #endregion

            return services;
        }
    }
}
