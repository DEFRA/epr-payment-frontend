using EPR.Payment.Portal.Common.RESTServices;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace EPR.Payment.Portal.Common.UnitTests.RESTServices
{
    public class TestableBaseHttpService : BaseHttpService
    {
        public TestableBaseHttpService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            string baseUrl,
            string endPointName,
            ITokenAcquisition tokenAcquisition,
            string downstreamScope,
            IFeatureManager featureManager)
            : base(httpContextAccessor, httpClientFactory, baseUrl, endPointName, tokenAcquisition, downstreamScope, featureManager)
        {
        }

        public string BaseUrl => _baseUrl;

        // Public wrappers for protected methods for testing purposes

        public Task<T> PublicGet<T>(string url, CancellationToken cancellationToken, bool includeTrailingSlash = true) =>
            Get<T>(url, cancellationToken, includeTrailingSlash);

        public Task<T> PublicPost<T>(string url, object payload, CancellationToken cancellationToken) =>
            Post<T>(url, payload, cancellationToken);

        public Task PublicPost(string url, object payload, CancellationToken cancellationToken) =>
            Post(url, payload, cancellationToken);

        // Additional methods for testing PUT and DELETE operations

        public Task<T> PublicPut<T>(string url, object payload, CancellationToken cancellationToken) =>
            Put<T>(url, payload, cancellationToken);

        public Task PublicPut(string url, object? payload, CancellationToken cancellationToken) =>
            Put(url, payload, cancellationToken);


        public Task<T> PublicDelete<T>(string url, object payload, CancellationToken cancellationToken) =>
            Delete<T>(url, payload, cancellationToken);

        public Task PublicDelete(string url, object payload, CancellationToken cancellationToken) =>
            Delete(url, payload, cancellationToken);

        // Wrapper for setting a Bearer token in the HttpClient
        public async Task PublicPrepareAuthenticatedClient() =>
            await PrepareAuthenticatedClient();

        public void PublicSetBearerToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token), "Bearer token cannot be null or empty.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
