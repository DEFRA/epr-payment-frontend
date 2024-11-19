using EPR.Payment.Portal.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EPR.Payment.Portal.Common.RESTServices
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseHttpService
    {
        protected readonly string _baseUrl;
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly string[] _scopes;
        private readonly IFeatureManager _featureManager;

        public BaseHttpService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            string baseUrl,
            string endPointName,
            ITokenAcquisition tokenAcquisition,
            string downstreamScope,
            IFeatureManager featureManager)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClient = httpClientFactory.CreateClient();

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            if (string.IsNullOrWhiteSpace(endPointName))
                throw new ArgumentNullException(nameof(endPointName));

            _baseUrl = baseUrl.TrimEnd('/') + "/" + endPointName;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
            _scopes = new[] { downstreamScope };
            _featureManager = featureManager;
        }

        protected async Task PrepareAuthenticatedClient()
        {
            if (await _featureManager.IsEnabledAsync("EnableAuthenticationFeature"))
            {
                try
                {
                    var token = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
                    if (string.IsNullOrEmpty(token))
                        throw new InvalidOperationException("Failed to acquire access token.");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to prepare the authenticated client.", ex);
                }
            }
            else
            {
                Console.WriteLine("Authentication is disabled. Skipping token acquisition.");
            }
        }

        protected async Task<T> Get<T>(string url, CancellationToken cancellationToken, bool includeTrailingSlash = true)
        {
            await PrepareAuthenticatedClient();

            var finalUrl = includeTrailingSlash ? $"{_baseUrl}/{url}/" : $"{_baseUrl}/{url}";
            return await Send<T>(CreateMessage(finalUrl, null, HttpMethod.Get), cancellationToken);
        }

        protected async Task<T> Post<T>(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            return await Send<T>(CreateMessage(finalUrl, payload, HttpMethod.Post), cancellationToken);
        }

        protected async Task Post(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url), "Value cannot be null or empty.");
            }

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            await Send(CreateMessage(finalUrl, payload, HttpMethod.Post), cancellationToken);
        }

        private HttpRequestMessage CreateMessage(string url, object? payload, HttpMethod method)
        {
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = method,
                Content = payload != null ? JsonContent.Create(payload) : null
            };
            return message;
        }

        private async Task<T> Send<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return IsValidJson(content) ? JsonConvert.DeserializeObject<T>(content)! : default!;
            }

            throw new ResponseCodeException(response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        private async Task Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error calling API: {response.StatusCode}");
        }


        protected async Task<T> Put<T>(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url), "Value cannot be null or empty.");
            }

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            return await Send<T>(CreateMessage(finalUrl, payload, HttpMethod.Put), cancellationToken);
        }

        protected async Task Put(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url), "Value cannot be null or empty.");
            }

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            await Send(CreateMessage(finalUrl, payload, HttpMethod.Put), cancellationToken);
        }


        protected async Task<T> Delete<T>(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "Value cannot be null or whitespace.");

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            return await Send<T>(CreateMessage(finalUrl, payload, HttpMethod.Delete), cancellationToken);
        }


        protected async Task Delete(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            await Send(CreateMessage(finalUrl, payload, HttpMethod.Delete), cancellationToken);
        }

        private bool IsValidJson(string stringValue)
        {
            try
            {
                JToken.Parse(stringValue);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
