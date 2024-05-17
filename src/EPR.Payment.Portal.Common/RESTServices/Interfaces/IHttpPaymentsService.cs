using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;

namespace EPR.Payment.Portal.Common.RESTServices.Interfaces
{
    public interface IHttpPaymentsService
    {
        Task<PaymentStatusResponseDto> GetPaymentStatus(string? paymentId);
        Task InsertPaymentStatus(string paymentId, PaymentStatusInsertRequestDto vm);
    }
}
