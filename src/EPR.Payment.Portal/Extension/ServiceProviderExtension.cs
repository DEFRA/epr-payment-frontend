using EPR.Common.Authorization.Extensions;
using EPR.Payment.Portal.Sessions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace EPR.Payment.Portal.Extension
{
    public static class ServiceProviderExtension
    {
        public static IServiceCollection RegisterAuthenication(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureAuthentication(services, configuration);
            ConfigureAuthorization(services, configuration);

            return services;
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    configuration.GetSection("AzureAdB2C").Bind(options);
                    options.ErrorPath = "/error";
                    options.ClaimActions.Add(new CorrelationClaimAction());
                }, options =>
                {
                    options.Cookie.Name = configuration.GetValue<string>("CookieOptions:AuthenticationCookieName");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("CookieOptions:AuthenticationExpiryInMinutes"));
                    options.SlidingExpiration = true;
                    options.Cookie.Path = "/";
                })
                .EnableTokenAcquisitionToCallDownstreamApi(new string[] { configuration.GetValue<string>("FacadeAPI:DownstreamScope") })
                .AddDistributedTokenCaches();
        }

        private static void ConfigureAuthorization(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterPolicy<JourneySession>(configuration);
        }
    }
}