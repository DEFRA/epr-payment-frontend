using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;

namespace EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces
{
    public interface IHttpPaymentFacade
    {
        Task<CompletePaymentResponseDto> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken);
        Task<string> InitiatePaymentAsync(PaymentRequestDto request, CancellationToken cancellationToken);
    }
}
