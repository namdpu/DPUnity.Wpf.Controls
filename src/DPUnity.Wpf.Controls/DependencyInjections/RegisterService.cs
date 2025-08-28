using DPUnity.Wpf.Controls.Controls.InputForms;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace DPUnity.Wpf.Controls.DependencyInjections
{
    public static class RegisterServices
    {
        public static IServiceCollection AddDPUControls(this IServiceCollection services)
        {
            services.AddScoped<DPInputService>();

            services.AddScoped<TextInputPage>();
            services.AddScoped<TextInputViewModel>();


            return services;
        }
    }
}
