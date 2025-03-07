﻿using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Common.RESTServices.Payments
{
    public class HttpPaymentFacadeHealthCheckService : IHttpPaymentFacadeHealthCheckService
    {
        private readonly HttpClient _httpClient;
        private readonly string _healthCheckUrl;

        public HttpPaymentFacadeHealthCheckService(
            IHttpClientFactory httpClientFactory,
            IOptions<FacadeService> config)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            if (config == null)
                throw new ArgumentNullException(nameof(config), "Config cannot be null.");

            if (string.IsNullOrWhiteSpace(config.Value.Url))
                throw new ArgumentNullException(nameof(config.Value.Url), "PaymentFacadeHealthCheck BaseUrl configuration is missing");

            _httpClient = httpClientFactory.CreateClient();
            _healthCheckUrl = $"{config.Value.Url.TrimEnd('/')}/admin/health";
        }

        public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.GetAsync(_healthCheckUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Health check request to {_healthCheckUrl} failed.", ex);
            }
        }
    }
}
