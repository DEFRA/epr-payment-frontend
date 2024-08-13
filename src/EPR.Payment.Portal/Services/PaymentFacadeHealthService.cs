using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Services.Interfaces;

namespace EPR.Payment.Portal.Services
{
    public class PaymentFacadeHealthService : IPaymentFacadeHealthService
    {
        protected readonly IHttpPaymentFacadeHealthCheckService _httpPaymentFacadeHealthCheckService;

        public PaymentFacadeHealthService(
            IHttpPaymentFacadeHealthCheckService httpPaymentServiceHealthCheckService)
        {
            _httpPaymentFacadeHealthCheckService = httpPaymentServiceHealthCheckService ?? throw new ArgumentNullException(nameof(httpPaymentServiceHealthCheckService));
        }

        public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken)
        {
            return await _httpPaymentFacadeHealthCheckService.GetHealthAsync(cancellationToken);
        }
    }
}
