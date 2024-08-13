using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Common.RESTServices.Payments
{
    public class HttpPaymentFacadeHealthCheckService : BaseHttpService, IHttpPaymentFacadeHealthCheckService
    {
        public HttpPaymentFacadeHealthCheckService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IOptions<FacadeService> config)
            : base(httpContextAccessor, httpClientFactory,
                config.Value.Url ?? throw new ArgumentNullException(nameof(config), "PaymentFacadeHealthCheck BaseUrl configuration is missing"),
                config.Value.EndPointName ?? throw new ArgumentNullException(nameof(config), "PaymentFacadeHealthCheck EndPointName configuration is missing"))
        {
        }

        public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken)
        {
            return await Get<HttpResponseMessage>(string.Empty, cancellationToken, false);
        }
    }
}
