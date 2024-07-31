using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Common.RESTServices.Payments
{
    public class HttpPaymentFacade : BaseHttpService, IHttpPaymentFacade
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _httpClientName;

        public HttpPaymentFacade(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IOptions<FacadeService> config)
            : base(httpContextAccessor, httpClientFactory,
                config.Value.Url ?? throw new ArgumentNullException(nameof(config), ExceptionMessages.PaymentFacadeBaseUrlMissing),
                config.Value.EndPointName ?? throw new ArgumentNullException(nameof(config), ExceptionMessages.PaymentFacadeEndPointNameMissing))
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClientName = config.Value.HttpClientName ?? throw new ArgumentNullException(nameof(config), ExceptionMessages.PaymentFacadeHttpClientNameMissing);
        }

        public async Task<CompletePaymentResponseDto> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken)
        {
            try
            {
                var url = UrlConstants.PaymentsComplete.Replace("{externalPaymentId}", externalPaymentId.ToString());
                var response = await Post<CompletePaymentResponseDto>(url, externalPaymentId, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionMessages.ErrorCompletePayment, ex);
            }
        }
    }
}
