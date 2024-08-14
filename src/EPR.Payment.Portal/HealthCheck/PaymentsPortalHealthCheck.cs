using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EPR.Payment.Portal.HealthCheck
{
    public class PaymentsPortalHealthCheck : IHealthCheck
    {
        public const string HealthCheckResultDescription = "Payments Portal Health Check";
        private readonly IPaymentFacadeHealthService _paymentFacadeHealthService;


        public PaymentsPortalHealthCheck(IPaymentFacadeHealthService paymentFacadeHealthService)
        {
            _paymentFacadeHealthService = paymentFacadeHealthService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _paymentFacadeHealthService.GetHealthAsync(cancellationToken);
                return response.IsSuccessStatusCode
                    ? HealthCheckResult.Healthy(HealthCheckResultDescription)
                    : HealthCheckResult.Unhealthy(HealthCheckResultDescription);
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
            }
        }
    }
}
