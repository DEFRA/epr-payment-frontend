namespace EPR.Payment.Portal.Services.Interfaces
{
    public interface IPaymentFacadeHealthService
    {
        Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken);
    }
}
