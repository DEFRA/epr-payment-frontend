using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace EPR.Payment.Portal.Helpers
{
    [ExcludeFromCodeCoverage]
    public class NoOpAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
#pragma warning disable CS0618 // Suppress the warning for ISystemClock being obsolete
        public NoOpAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder,
            ISystemClock clock) // ISystemClock is still required by AuthenticationHandler
            : base(options, logger, encoder, clock)
        {
        }
#pragma warning restore CS0618 // Re-enable warnings

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "NoOpUser") };
            var identity = new ClaimsIdentity(claims, "NoOpScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "NoOpScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}