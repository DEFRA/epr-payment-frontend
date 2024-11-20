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
            string tenant,
            string userFlow = null,
            string tokenAcquisitionOptionsName = null) => Task.FromResult(string.Empty);

        public Task<string> GetAccessTokenForUserAsync(string[] scopes, TokenAcquisitionOptions options) => Task.FromResult(string.Empty);

        public void ReplyForbiddenWithWwwAuthenticateHeader(IEnumerable<string> scopes, string userFlow = null)
        {
            // No-op
        }

        public Task<string> GetAccessTokenForUserAsync(
            IEnumerable<string> scopes,
            string? tenantId = null,
            string? userFlow = null,
            string? tokenAcquisitionOptionsName = null,
            ClaimsPrincipal? user = null,
            TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<AuthenticationResult> GetAuthenticationResultForUserAsync(IEnumerable<string> scopes, string? authenticationScheme, string? tenantId = null, string? userFlow = null, ClaimsPrincipal? user = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenForAppAsync(string scope, string? authenticationScheme, string? tenant = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> GetAuthenticationResultForAppAsync(string scope, string? authenticationScheme, string? tenant = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            throw new NotImplementedException();
        }

        public void ReplyForbiddenWithWwwAuthenticateHeader(IEnumerable<string> scopes, MsalUiRequiredException msalServiceException, string? authenticationScheme, HttpResponse? httpResponse = null)
        {
            throw new NotImplementedException();
        }

        public string GetEffectiveAuthenticationScheme(string? authenticationScheme)
        {
            throw new NotImplementedException();
        }

        public Task ReplyForbiddenWithWwwAuthenticateHeaderAsync(IEnumerable<string> scopes, MsalUiRequiredException msalServiceException, HttpResponse? httpResponse = null)
        {
            throw new NotImplementedException();
        }
    }
}
