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
    [ExcludeFromCodeCoverage] // Excluding only because sonar qube is complaining about the lines already covered by tests.
    public abstract class BaseHttpService
    {
        protected readonly string _baseUrl;
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly string[] _scopes;
        private readonly IFeatureManager _featureManager;

        protected BaseHttpService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            string baseUrl,
            string endPointName,
            ITokenAcquisition tokenAcquisition,
            string downstreamScope,
            IFeatureManager featureManager)
        {
            _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition)); // Moved up
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));

            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl), "Base URL is required.");

            if (string.IsNullOrWhiteSpace(endPointName))
                throw new ArgumentNullException(nameof(endPointName), "Endpoint name is required.");

            _baseUrl = baseUrl.TrimEnd('/') + "/" + endPointName;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _scopes = new[] { downstreamScope ?? throw new ArgumentNullException(nameof(downstreamScope)) };
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
        }

        /// <summary>
        /// Performs an Http GET returning the specified object
        /// </summary>
        protected async Task<T> Get<T>(string url, CancellationToken cancellationToken, bool includeTrailingSlash = true)
        {
            await PrepareAuthenticatedClient();

            var finalUrl = includeTrailingSlash ? $"{_baseUrl}/{url}/" : $"{_baseUrl}/{url}";
            return await Send<T>(CreateMessage(finalUrl, null, HttpMethod.Get), cancellationToken);
        }

        /// <summary>
        /// Performs an Http POST returning the specified object
        /// </summary>
        protected async Task<T> Post<T>(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            return await Send<T>(CreateMessage(finalUrl, payload, HttpMethod.Post), cancellationToken);
        }

        /// <summary>
        /// Performs an Http POST without returning any data
        /// </summary>
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

        /// <summary>
        /// Performs an Http PUT returning the specified object
        /// </summary>
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

        /// <summary>
        /// Performs an Http PUT without returning any data
        /// </summary>
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

        /// <summary>
        /// Performs an Http DELETE returning the specified object
        /// </summary>
        protected async Task<T> Delete<T>(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "Value cannot be null or whitespace.");

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            return await Send<T>(CreateMessage(finalUrl, payload, HttpMethod.Delete), cancellationToken);
        }

        /// <summary>
        /// Performs an Http DELETE without returning any data
        /// </summary>
        protected async Task Delete(string url, object? payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

            await PrepareAuthenticatedClient();

            var finalUrl = $"{_baseUrl}/{url}/";
            await Send(CreateMessage(finalUrl, payload, HttpMethod.Delete), cancellationToken);
        }

        private static HttpRequestMessage CreateMessage(
            string url,
            object? payload,
            HttpMethod httpMethod)
        {
            var msg = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = httpMethod
            };

            if (payload != null)
            {
                msg.Content = JsonContent.Create(payload);
            }

            return msg;
        }

        private static bool IsValidJson(string stringValue)
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

        private async Task<T> Send<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var streamReader = new StreamReader(responseStream);
                var content = await streamReader.ReadToEndAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(content))
                    return default!;

                return ReturnValue<T>(content);
            }
            else
            {
                // get any message from the response
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var content = default(string);

                if (responseStream.Length > 0)
                {
                    using var streamReader = new StreamReader(responseStream);
                    content = await streamReader.ReadToEndAsync(cancellationToken);
                }

                // set the response status code and throw the exception for the middleware to handle
                throw new ResponseCodeException(response.StatusCode, content!);
            }
        }

        private async Task Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _httpContextAccessor.HttpContext!.Response.StatusCode = (int)response.StatusCode;
                // for now we don't know how we're going to handle errors specifically,
                // so we'll just throw an error with the error code
#pragma warning disable S112
                throw new Exception($"Error occurred calling API with error code: {response.StatusCode}. Message: {response.ReasonPhrase}");
#pragma warning restore S112
            }
        }

        private T ReturnValue<T>(string value)
        {
            if (IsValidJson(value))
                return JsonConvert.DeserializeObject<T>(value)!;
            else
                return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}