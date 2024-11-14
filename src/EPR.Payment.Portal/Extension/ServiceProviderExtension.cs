using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Sessions;
using Microsoft.AspNetCore.Localization;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Extension
{
    [ExcludeFromCodeCoverage]
    public static class ServiceProviderExtension
    {
        public static IServiceCollection RegisterWebComponents(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureLocalization(services);
            return services;
        }

        private static void ConfigureLocalization(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources")
                .Configure<RequestLocalizationOptions>(options =>
                {
                    var cultureList = new[] { Language.English, Language.Welsh };
                    options.SetDefaultCulture(Language.English);
                    options.AddSupportedCultures(cultureList);
                    options.AddSupportedUICultures(cultureList);
                    options.RequestCultureProviders = new IRequestCultureProvider[]
                    {
                    new SessionRequestCultureProvider(),
                    };
                });
        }
    }
}
