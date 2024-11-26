using EPR.Common.Authorization.Extensions;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Helpers;
using EPR.Payment.Portal.Options;
using EPR.Payment.Portal.Sessions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using StackExchange.Redis; // Required for Redis-based DataProtection
using System.Diagnostics.CodeAnalysis;

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
            ConfigureDataProtection(services);
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
            services.Configure<EPR.Payment.Portal.Common.Options.CookieOptions>(configuration.GetSection(EPR.Payment.Portal.Common.Options.CookieOptions.ConfigSection));
            services.Configure<RedisOptions>(configuration.GetSection(EPR.Payment.Portal.Common.Options.RedisOptions.ConfigSection));
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();
            var cookieOptions = sp.GetRequiredService<IOptions<EPR.Payment.Portal.Common.Options.CookieOptions>>().Value;
            var featureManager = sp.GetRequiredService<IFeatureManager>();
            var facadeApiOptions = sp.GetRequiredService<IOptions<AccountsFacadeApiOptions>>().Value;

            if (featureManager.IsEnabledAsync("EnableAuthenticationFeature").GetAwaiter().GetResult())
            {
                // Authentication is enabled
                services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(
                        options =>
                        {
                            configuration.GetSection(AzureAdB2COptions.ConfigSection).Bind(options);

                            options.CorrelationCookie.Name = cookieOptions.CorrelationCookieName;
                            options.NonceCookie.Name = cookieOptions.OpenIdCookieName;
                            options.ErrorPath = "/error";
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
            else
            {
                services.AddAuthentication("NoOpScheme")
                    .AddScheme<AuthenticationSchemeOptions, NoOpAuthenticationHandler>("NoOpScheme", options => { });
                services.AddTransient<ITokenAcquisition, NoOpTokenAcquisition>();
            }
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

        private static void ConfigureDataProtection(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var globalVariables = sp.GetRequiredService<IOptions<GlobalVariables>>().Value;

            if (!globalVariables.UseLocalSession)
            {
                // Retrieve Redis configuration with validation
                var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
                var redisConnectionString = redisOptions.ConnectionString;
                var redisInstanceName = redisOptions.InstanceName;

                if (string.IsNullOrWhiteSpace(redisConnectionString))
                {
                    throw new InvalidOperationException("Redis connection string is not configured.");
                }

                if (string.IsNullOrWhiteSpace(redisInstanceName))
                {
                    throw new InvalidOperationException("Redis instance name is not configured.");
                }

                // Configure Data Protection with Redis persistence
                services.AddDataProtection()
                .SetApplicationName("EprProducers")
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnectionString), "DataProtection-Keys");
            }
            else
            {
                // Local development fallback
                services.AddDataProtection();
            }
        }

    }
}
