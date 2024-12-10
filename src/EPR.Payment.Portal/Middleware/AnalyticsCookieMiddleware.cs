using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Constants;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Middleware
{
    public class AnalyticsCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public AnalyticsCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            ICookieService cookieService,
            IOptions<GoogleAnalyticsOptions> googleAnalyticsOptions)
        {
            httpContext.Items[ContextKeys.UseGoogleAnalyticsCookieKey] = cookieService.HasUserAcceptedCookies(httpContext.Request.Cookies);
            httpContext.Items[ContextKeys.TagManagerContainerIdKey] = googleAnalyticsOptions.Value.TagManagerContainerId;

            await _next(httpContext);
        }
    }
}
