using AutoMapper;
using EPR.Payment.Portal.Common.Profiles;
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
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(FeesProfile));
            services
                .AddHttpContextAccessor()
                .AddHttpClient()
                .AddScoped<IFeesService, FeesService>()
                .AddScoped<IHttpFeesService, HttpFeesService>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new FeesProfile());
                mc.AllowNullCollections = true;
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddRazorPages()
            .AddViewOptions(o =>
            {
                    o.HtmlHelperOptions.ClientValidationEnabled = configuration.GetValue<bool>("ClientValidationEnabled");
                });

            return services;

        }
    }
}
