using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EPR.Payment.Portal.Common.RESTServices
{
    public class HttpPaymentsService : BaseHttpService, IHttpPaymentsService
    {
        public HttpPaymentsService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
            : base(httpContextAccessor, httpClientFactory,
                configuration?["PaymentFacade:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BaseUrl configuration is missing"),
                configuration?["PaymentFacade:EndPointName"] ?? throw new ArgumentNullException(nameof(configuration), "EndPointName configuration is missing"))
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<PaymentStatusResponseDto> GetPaymentStatus(string? paymentId)
        {
            return await Get<PaymentStatusResponseDto>($"{paymentId}/status");
        }

        public async Task InsertPaymentStatus(string paymentId, PaymentStatusInsertRequestDto request)
        {
            await Post($"{paymentId}/status", request);
        }
    }
}
