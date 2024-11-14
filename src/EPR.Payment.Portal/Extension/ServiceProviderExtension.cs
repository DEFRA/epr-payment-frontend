using EPR.Common.Authorization.Extensions;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Options;
using EPR.Payment.Portal.Sessions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using CookieOptions = EPR.Payment.Portal.Common.Options.CookieOptions;

namespace EPR.Payment.Portal.Extension
{
    [ExcludeFromCodeCoverage]
    public static class ServiceProviderExtension
    {
        public static IServiceCollection RegisterWebComponents(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureOptions(services, configuration);
            ConfigureLocalization(services);
            ConfigureAuthentication(services, configuration);
            ConfigureAuthorization(services, configuration);
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

        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookieOptions>(configuration.GetSection(CookieOptions.ConfigSection));
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();
            var cookieOptions = sp.GetRequiredService<IOptions<CookieOptions>>().Value;
            var facadeApiOptions = sp.GetRequiredService<IOptions<AccountsFacadeApiOptions>>().Value;

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(
                    options =>
                    {
                        configuration.GetSection(AzureAdB2COptions.ConfigSection).Bind(options);

                        options.CorrelationCookie.Name = cookieOptions.CorrelationCookieName;
                        options.NonceCookie.Name = cookieOptions.OpenIdCookieName;
                        options.ErrorPath = "/error";
                        options.ClaimActions.Add(new CorrelationClaimAction());
                    },
                    options =>
                    {
                        options.Cookie.Name = cookieOptions.AuthenticationCookieName;
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieOptions.AuthenticationExpiryInMinutes);
                        options.SlidingExpiration = true;
                        options.Cookie.Path = "/";
                    })
                .EnableTokenAcquisitionToCallDownstreamApi(new[] { facadeApiOptions.DownstreamScope })
                .AddDistributedTokenCaches();
        }

        private static void ConfigureAuthorization(IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;

                var azureB2COptions = services.BuildServiceProvider().GetRequiredService<IOptions<AzureAdB2COptions>>().Value;

                options.LoginPath = azureB2COptions.SignedOutCallbackPath;
                options.AccessDeniedPath = azureB2COptions.SignedOutCallbackPath;

                options.SlidingExpiration = true;
            });

            services.RegisterPolicy<PaymentPortalSession>(configuration);
        }
    }
}
