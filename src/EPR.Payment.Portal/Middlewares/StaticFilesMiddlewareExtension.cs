using Microsoft.Extensions.FileProviders;

namespace EPR.Payment.Portal.Middlewares;

public static class StaticFilesMiddlewareExtension
{
    public static IApplicationBuilder UseStaticFilesMiddlewareExtension(this IApplicationBuilder builder)
    {
        builder.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Asset")),
                RequestPath = new PathString("/Asset"),
            });

        builder.Use(async (context, next) =>
        {
            await next(context);
        });

        return builder;
    }
}