using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Exceptions;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;

namespace EPR.Payment.Portal.Common.RESTServices.Payments
{
    public class HttpPaymentFacadeV2(
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory,
        ITokenAcquisition tokenAcquisition,
        IOptions<FacadeServiceV2> config,
        IFeatureManager featureManager)
        : BaseHttpService(httpContextAccessor,
            httpClientFactory,
            config.Value.Url ?? throw new ArgumentNullException(nameof(config), "Base URL is missing."),
            config.Value.EndPointName ?? throw new ArgumentNullException(nameof(config), "Endpoint Name is missing."),
            tokenAcquisition,
            config.Value.DownstreamScope ??
            throw new ArgumentNullException(nameof(config), "Downstream Scope is missing."),
            featureManager), IHttpPaymentFacadeV2
    {
        public async Task<CompletePaymentResponseDto> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken)
        {
            try
            {
                var url = UrlConstants.OnlinePaymentsComplete.Replace("{externalPaymentId}", externalPaymentId.ToString());
                return await Post<CompletePaymentResponseDto>(url, externalPaymentId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ExceptionMessages.ErrorCompletePayment, ex);
            }
        }

        public async Task<string> InitiatePaymentAsync(PaymentRequestDto? request, CancellationToken cancellationToken)
        {
            try
            {
                return await Post<string>(UrlConstants.OnlinePaymentsInitiate, request, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ExceptionMessages.ErrorInitiatePayment, ex);
            }
        }
    }
}
