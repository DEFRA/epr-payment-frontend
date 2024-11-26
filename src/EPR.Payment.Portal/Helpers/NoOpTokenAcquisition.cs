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
            string? authenticationScheme = null, // Ensure this matches the interface
            string? tenantId = null, // Correct name with default value
            string? tokenAcquisitionOptionsName = null) // Ensure this matches the interface
        {
            return Task.FromResult(string.Empty);
        }

        public Task<string> GetAccessTokenForUserAsync(string[] scopes, TokenAcquisitionOptions? options) => Task.FromResult(string.Empty);

        public void ReplyForbiddenWithWwwAuthenticateHeader(IEnumerable<string> scopes, string? tenantId = null) // Correct parameter name and default value
        {
            // No-op
        }

        public Task<string> GetAccessTokenForUserAsync(
            IEnumerable<string> scopes,
            string? authenticationScheme = null, // Correct name with default value
            string? tenantId = null, // Correct name with default value
            string? tokenAcquisitionOptionsName = null, // Correct name with default value
            ClaimsPrincipal? user = null, // Add default value
            TokenAcquisitionOptions? tokenAcquisitionOptions = null) // Add default value
        {
            return Task.FromResult(string.Empty);
        }

        public Task<AuthenticationResult> GetAuthenticationResultForUserAsync(
            IEnumerable<string> scopes,
            string? authenticationScheme = null, // Add default value
            string? tenantId = null, // Correct name with default value
            string? userFlow = null, // Add default value if required
            ClaimsPrincipal? user = null, // Add default value
            TokenAcquisitionOptions? tokenAcquisitionOptions = null) // Add default value
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenForAppAsync(
            string scope,
            string? authenticationScheme = null, // Add default value
            string? tenantId = null, // Correct name with default value
            TokenAcquisitionOptions? tokenAcquisitionOptions = null) // Add default value
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> GetAuthenticationResultForAppAsync(
            string scope,
            string? authenticationScheme = null, // Add default value
            string? tenantId = null, // Correct name with default value
            TokenAcquisitionOptions? tokenAcquisitionOptions = null) // Add default value
        {
            throw new NotImplementedException();
        }

        public void ReplyForbiddenWithWwwAuthenticateHeader(
            IEnumerable<string> scopes,
            MsalUiRequiredException msalServiceException,
            string? authenticationScheme = null, // Add default value
            HttpResponse? httpResponse = null) // Add default value
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
            HttpResponse? httpResponse = null) // Add default value
        {
            throw new NotImplementedException();
        }
    }
}