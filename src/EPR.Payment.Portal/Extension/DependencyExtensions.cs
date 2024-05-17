using EPR.Payment.Portal.Common.RESTServices;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
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
            services
                .AddScoped<IPaymentsService, PaymentsService>()
                .AddScoped<IHttpPaymentsService, HttpPaymentsService>()
                .AddScoped<IHttpPaymentsService, HttpPaymentsService>()
                .AddScoped<IPaymentsService, PaymentsService>();
            services.AddHttpContextAccessor();
            services.AddHttpClient();

            return services;

        }
    }
}
