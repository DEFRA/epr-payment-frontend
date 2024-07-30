using EPR.Payment.Portal.Common.Models;

namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface IPaymentsServices
    {
        Task<CompletePaymentViewModel> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken);
    }
}
