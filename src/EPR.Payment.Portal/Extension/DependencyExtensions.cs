using EPR.Payment.Portal.Services;
using EPR.Payment.Portal.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Extension
{
    [ExcludeFromCodeCoverage]
    public static class DependencyExtensions
    {
        public static IServiceCollection AddDependencies(
            this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpContextAccessor();
            services.AddScoped<IPaymentsServices, PaymentsService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();

            return services;

        }
    }
}
