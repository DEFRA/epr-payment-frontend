using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Helpers
{
    [ExcludeFromCodeCoverage] // Excluding only because sonar qube is complaining about the lines already covered by tests.
    public static class DependencyHelper
    {
        public static IServiceCollection AddPortalDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<FacadeConfiguration>(configuration.GetSection(FacadeConfiguration.SectionName));

            RegisterHttpService<IHttpPaymentFacade, HttpPaymentFacade>(
                services, nameof(FacadeConfiguration.FacadeService));

            return services;
        }

        private static void RegisterHttpService<TInterface, TImplementation>(
            IServiceCollection services, string configName, string? endPointOverride = null)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            // Perform validation before adding to the service collection
            var serviceOptions = CreateServiceOptions(services, configName, endPointOverride);

            services.AddScoped(s =>
            {
                Trace.TraceInformation($"Registering service {typeof(TImplementation).Name} for {configName}");

                object? instance = Activator.CreateInstance(typeof(TImplementation),
                        s.GetRequiredService<IHttpContextAccessor>(),
                        s.GetRequiredService<IHttpClientFactory>(),
                        s.GetRequiredService<Microsoft.Identity.Web.ITokenAcquisition>(),
                        serviceOptions,
                        s.GetRequiredService<IFeatureManager>());
                

                Trace.TraceError(instance == null ? $"Failed to create instance of {typeof(TImplementation).Name}" : $"Successfully created instance of {typeof(TImplementation).Name}");

                return instance == null
                    ? throw new InvalidOperationException($"Failed to create instance of {typeof(TImplementation).Name}")
                    : (TInterface)(TImplementation)instance;
            });
        }

        private static IOptions<FacadeService> CreateServiceOptions(IServiceCollection services, string configName, string? endPointOverride)
        {
            var serviceProvider = services.BuildServiceProvider();
            var servicesConfig = serviceProvider.GetRequiredService<IOptions<FacadeConfiguration>>().Value;

            var serviceConfig = (FacadeService?)servicesConfig.GetType().GetProperty(configName)?.GetValue(servicesConfig);

            ValidateServiceConfiguration(serviceConfig, configName);

            var endPointName = endPointOverride ?? serviceConfig?.EndPointName;

            return Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = serviceConfig?.Url,
                EndPointName = endPointName,
                HttpClientName = serviceConfig?.HttpClientName,
                DownstreamScope = serviceConfig?.DownstreamScope
            });
        }

        private static void ValidateServiceConfiguration(FacadeService? serviceConfig, string configName)
        {
            if (serviceConfig?.Url == null)
            {
                throw new InvalidOperationException($"{configName} Url configuration is missing.");
            }

            if (serviceConfig.EndPointName == null)
            {
                throw new InvalidOperationException($"{configName} EndPointName configuration is missing.");
            }

            if (serviceConfig.DownstreamScope == null)
            {
                throw new InvalidOperationException($"{configName} DownstreamScope configuration is missing.");
            }
        }
    }
}
