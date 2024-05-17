using AutoMapper;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Models.Request;
using EPR.Payment.Portal.Common.Models.Response;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services.Interfaces;

namespace EPR.Payment.Portal.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IHttpPaymentsService _httpPaymentsService;
        private IMapper _mapper;
        public PaymentsService(IMapper mapper, IHttpPaymentsService httpPaymentsService)
        {
            _mapper = mapper;
            _httpPaymentsService = httpPaymentsService ?? throw new ArgumentNullException(nameof(httpPaymentsService));
        }
        public async Task<PaymentStatusResponseViewModel> GetPaymentStatus(string? paymentId)
        {
            var response = await _httpPaymentsService.GetPaymentStatus(paymentId);
            return _mapper.Map<PaymentStatusResponseViewModel>(response);
        }

        public async Task InsertPaymentStatus(string paymentId, PaymentStatusInsertRequestViewModel vm)
        {
            var dto = _mapper.Map<PaymentStatusInsertRequestDto>(vm);
            await _httpPaymentsService.InsertPaymentStatus(paymentId, dto);
        }
    }
}
