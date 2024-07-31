using AutoMapper;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Services.Interfaces;

namespace EPR.Payment.Portal.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IHttpPaymentFacade _httpPaymentFacade;
        private IMapper _mapper;

        public PaymentsService(IMapper mapper, IHttpPaymentFacade httpPaymentFacade)
        {
            _mapper = mapper;
            _httpPaymentFacade = httpPaymentFacade ?? throw new ArgumentNullException(nameof(httpPaymentFacade));
        }
        public async Task<CompletePaymentViewModel> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken)
        {
            if (externalPaymentId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ErrorEcternalPaymentIdEmpty);
            }
            try
            {
                var completePaymentResponseDto = await _httpPaymentFacade.CompletePaymentAsync(externalPaymentId, cancellationToken);
                if (string.IsNullOrEmpty(completePaymentResponseDto.Reference))
                {
                    throw new Exception(ExceptionMessages.PaymentDataNotFound);
                }
                var viewModel = _mapper.Map<CompletePaymentViewModel>(completePaymentResponseDto);
                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionMessages.ErrorRetrievingCompletePayment, ex);
            }
        }
    }
}
