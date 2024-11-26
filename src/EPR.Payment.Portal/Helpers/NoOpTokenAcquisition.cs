using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace EPR.Payment.Portal.Helpers
{
    [ExcludeFromCodeCoverage]
    public class NoOpTokenAcquisition : ITokenAcquisition
    {
        public Task<string> GetAccessTokenForAppAsync(string scope) => Task.FromResult(string.Empty);

        public Task<string> GetAccessTokenForUserAsync(string[] scopes) => Task.FromResult(string.Empty);

        public Task<string> GetAccessTokenForUserAsync(
            string[] scopes,
            string? authenticationScheme,
            string? userFlow,
            string? tokenAcquisitionOptionsName)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<string> GetAccessTokenForUserAsync(string[] scopes, TokenAcquisitionOptions? options) => Task.FromResult(string.Empty);

        public void ReplyForbiddenWithWwwAuthenticateHeader(IEnumerable<string> scopes, string? userFlow)
        {
            // No-op
        }

        public Task<string> GetAccessTokenForUserAsync(
            IEnumerable<string> scopes,
            string? authenticationScheme,
            string? userFlow,
            string? tokenAcquisitionOptionsName,
            ClaimsPrincipal? user,
            TokenAcquisitionOptions? tokenAcquisitionOptions)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<AuthenticationResult> GetAuthenticationResultForUserAsync(
            IEnumerable<string> scopes,
            string? authenticationScheme,
            string? tenantId,
            string? userFlow,
            ClaimsPrincipal? user,
            TokenAcquisitionOptions? tokenAcquisitionOptions)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenForAppAsync(
            string scope,
            string? authenticationScheme,
            string? tenantId,
            TokenAcquisitionOptions? tokenAcquisitionOptions)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> GetAuthenticationResultForAppAsync(
            string scope,
            string? authenticationScheme,
            string? tenantId,
            TokenAcquisitionOptions? tokenAcquisitionOptions)
        {
            throw new NotImplementedException();
        }

        public void ReplyForbiddenWithWwwAuthenticateHeader(
            IEnumerable<string> scopes,
            MsalUiRequiredException msalServiceException,
            string? authenticationScheme,
            HttpResponse? httpResponse)
        {
            throw new NotImplementedException();
        }

        public string GetEffectiveAuthenticationScheme(string? authenticationScheme)
        {
            throw new NotImplementedException();
        }

        public Task ReplyForbiddenWithWwwAuthenticateHeaderAsync(
            IEnumerable<string> scopes,
            MsalUiRequiredException msalServiceException,
            HttpResponse? httpResponse)
        {
            throw new NotImplementedException();
        }
    }
}
