using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;

namespace EPR.Payment.Portal.Common.RESTServices.Payments
{
    public class HttpPaymentFacadeHealthCheckService : BaseHttpService, IHttpPaymentFacadeHealthCheckService
    {
        public HttpPaymentFacadeHealthCheckService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            ITokenAcquisition tokenAcquisition,
            IOptions<FacadeService> config,
            IFeatureManager featureManager)
    : base(
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)),
        httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory)),
        config?.Value.Url ?? throw new ArgumentNullException(nameof(config), "PaymentFacadeHealthCheck BaseUrl configuration is missing"),
        config.Value.EndPointName ?? throw new ArgumentNullException(nameof(config), "PaymentFacadeHealthCheck EndPointName configuration is missing"),
        tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition)),
        config.Value.DownstreamScope ?? throw new ArgumentNullException(nameof(config), "PaymentFacadeHealthCheck DownstreamScope configuration is missing"),
        featureManager ?? throw new ArgumentNullException(nameof(featureManager)))
        {
        }

        public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken)
        {
            await PrepareAuthenticatedClient();
            return await Get<HttpResponseMessage>(string.Empty, cancellationToken, false);
        }
    }
}
