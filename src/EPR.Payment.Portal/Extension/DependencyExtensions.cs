using EPR.Payment.Portal.HealthCheck;
using EPR.Payment.Portal.Services;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Extension
{
    [ExcludeFromCodeCoverage]
    public static class DependencyExtensions
    {
        private static readonly string Ready = "ready";
        public static IServiceCollection AddDependencies(
            this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpContextAccessor();
            services.AddScoped<IPaymentsService, PaymentsService>();
            services.AddScoped<IPaymentFacadeHealthService, PaymentFacadeHealthService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddHttpClient();

            return services;

        }

        public static IServiceCollection AddServiceHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<PaymentsPortalHealthCheck>(PaymentsPortalHealthCheck.HealthCheckResultDescription,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { Ready });
            return services;
        }
    }
}
