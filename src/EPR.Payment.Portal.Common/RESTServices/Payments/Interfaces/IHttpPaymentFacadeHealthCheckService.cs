namespace EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces
{
    public interface IHttpPaymentFacadeHealthCheckService
    {
        Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken);
    }
}
