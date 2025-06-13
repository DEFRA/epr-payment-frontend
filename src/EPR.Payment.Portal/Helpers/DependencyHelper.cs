using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Identity.Web;

namespace EPR.Payment.Portal.Helpers;

[ExcludeFromCodeCoverage] // Excluding only because sonar qube is complaining about the lines already covered by tests.
public static class DependencyHelper
{
    public static IServiceCollection AddPortalDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FacadeConfiguration>(configuration.GetSection(FacadeConfiguration.SectionName));

        RegisterHttpService<IHttpPaymentFacade, HttpPaymentFacade, FacadeService>(
            services, nameof(FacadeConfiguration.FacadeService));

        RegisterHttpService<IHttpPaymentFacadeV2, HttpPaymentFacadeV2, FacadeServiceV2>(
            services, nameof(FacadeConfiguration.FacadeServiceV2));

        RegisterHttpService<IHttpPaymentFacadeHealthCheckService, HttpPaymentFacadeHealthCheckService, FacadeService>(
            services, nameof(FacadeConfiguration.FacadeService), (type, options, serviceProvider) =>
                // Special case: Health check service does not require authentication or context accessor
                Activator.CreateInstance(type, serviceProvider.GetRequiredService<IHttpClientFactory>(), options), "health");

        return services;
    }

    private static void RegisterHttpService<TInterface, TImplementation, TFacadeService>(
        IServiceCollection services, string configName,
        string? endPointOverride = null)
        where TInterface : class
        where TImplementation : class, TInterface
        where TFacadeService : FacadeService, new()
    {
        RegisterHttpService<TInterface, TImplementation, TFacadeService>(services, configName, CreateInstance, endPointOverride);
    }

    private static void RegisterHttpService<TInterface, TImplementation, TFacadeService>(
        IServiceCollection services, string configName, Func<Type, IOptions<TFacadeService>, IServiceProvider, object?>? createInstance, string? endPointOverride = null)
        where TInterface : class
        where TImplementation : class, TInterface
        where TFacadeService : FacadeService, new()
    {
        // Perform validation before adding to the service collection
        var serviceOptions = CreateServiceOptions<TFacadeService>(services, configName, endPointOverride);

        services.AddScoped(s =>
        {
            Trace.TraceInformation($"Registering service {typeof(TImplementation).Name} for {configName}");
                
            var instance = createInstance?.Invoke(typeof(TImplementation), serviceOptions, s);
                
            Trace.TraceError(instance == null ? $"Failed to create instance of {typeof(TImplementation).Name}" : $"Successfully created instance of {typeof(TImplementation).Name}");

            return instance == null
                ? throw new InvalidOperationException($"Failed to create instance of {typeof(TImplementation).Name}")
                : (TInterface)(TImplementation)instance;
        });
    }

    private static IOptions<T> CreateServiceOptions<T>(IServiceCollection services, string configName, string? endPointOverride)
        where T: FacadeService, new()
    {
        var serviceProvider = services.BuildServiceProvider();
        var servicesConfig = serviceProvider.GetRequiredService<IOptions<FacadeConfiguration>>().Value;

        var serviceConfig = (T?)servicesConfig.GetType().GetProperty(configName)?.GetValue(servicesConfig);

        ValidateServiceConfiguration(serviceConfig, configName);

        var endPointName = endPointOverride ?? serviceConfig?.EndPointName;

        return Microsoft.Extensions.Options.Options.Create(new T
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
        
    private static object? CreateInstance(Type type, IOptions<FacadeService> options, IServiceProvider serviceProvider)
    {
        return Activator.CreateInstance(type,
            serviceProvider.GetRequiredService<IHttpContextAccessor>(),
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            serviceProvider.GetRequiredService<ITokenAcquisition>(),
            options,
            serviceProvider.GetRequiredService<IFeatureManager>());
    }
}