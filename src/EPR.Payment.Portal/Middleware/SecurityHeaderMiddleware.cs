namespace EPR.Payment.Portal.Middleware;

using System.Security.Cryptography;
using Constants;

public class SecurityHeaderMiddleware
{
    private const int DefaultBytesInNonce = 32;

    private readonly RequestDelegate _next;
    private readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

    public SecurityHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, IConfiguration configuration)
    {
        var scriptNonce = GenerateNonce();

        var whitelistedFormActionAddresses = configuration["AzureAdB2C:Instance"];

        if (string.IsNullOrEmpty(whitelistedFormActionAddresses))
        {
            throw new InvalidOperationException("AzureAdB2C:Instance is not configured.");
        }

        const string permissionsPolicy =
            "accelerometer=(),ambient-light-sensor=(),autoplay=(),battery=(),camera=(),display-capture=()," +
            "document-domain=(),encrypted-media=(),fullscreen=(),gamepad=(),geolocation=(),gyroscope=()," +
            "layout-animations=(self),legacy-image-formats=(self),magnetometer=(),microphone=(),midi=()," +
            "oversized-images=(self),payment=(),picture-in-picture=(),publickey-credentials-get=(),speaker-selection=()," +
            "sync-xhr=(self),unoptimized-images=(self),unsized-media=(self),usb=(),screen-wake-lock=(),web-share=(),xr-spatial-tracking=()";

        httpContext.Response.Headers.ContentSecurityPolicy = GetContentSecurityPolicyHeader();
        
        httpContext.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        httpContext.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        httpContext.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
        httpContext.Response.Headers.Append("Permissions-Policy", permissionsPolicy);
        httpContext.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        httpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        httpContext.Response.Headers.Append("X-Frame-Options", "deny");
        httpContext.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
        httpContext.Response.Headers.Append("X-Robots-Tag", "noindex, nofollow");

        httpContext.Items[ContextKeys.ScriptNonceKey] = scriptNonce;

        await _next(httpContext);
    }

    private static string GetContentSecurityPolicyHeader()
    {
        const string baseUri = "base-uri 'none'";
        const string requireTrustedTypes = "require-trusted-types-for 'script'";

        return string.Join(";", baseUri, requireTrustedTypes);
    }

    private string GenerateNonce()
    {
        var bytes = new byte[DefaultBytesInNonce];
        _random.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}