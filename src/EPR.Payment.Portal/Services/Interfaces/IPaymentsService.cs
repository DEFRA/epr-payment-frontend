using EPR.Payment.Portal.Common.Models;

namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task<CompletePaymentViewModel> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken);
    }
}
