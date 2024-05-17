using EPR.Payment.Portal.Common.Models.Request;
using EPR.Payment.Portal.Common.Models.Response;

namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task<PaymentStatusResponseViewModel> GetPaymentStatus(string? paymentId);
        Task InsertPaymentStatus(string paymentId, PaymentStatusInsertRequestViewModel vm);
    }
}
