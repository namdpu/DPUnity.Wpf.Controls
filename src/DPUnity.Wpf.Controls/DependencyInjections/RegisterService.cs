using DPUnity.Wpf.Controls.Controls.InputForms;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace DPUnity.Wpf.Controls.DependencyInjections
{
    public static class RegisterServices
    {
        public static IServiceCollection AddDPUControls(this IServiceCollection services)
        {
            services.AddScoped<IDPInputService, DPInputService>();

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

            services.AddScoped<ProcessPage>();
            #endregion

            return services;
        }
    }
}
