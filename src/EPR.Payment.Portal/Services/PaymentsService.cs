using AutoMapper;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Exceptions;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Services.Interfaces;

namespace EPR.Payment.Portal.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IHttpPaymentFacade _httpPaymentFacade;
        private readonly IHttpPaymentFacadeV2 _httpPaymentFacadeV2;
        private readonly ILogger<PaymentsService> _logger;
        private readonly IMapper _mapper;

        public PaymentsService(IMapper mapper,
            IHttpPaymentFacade httpPaymentFacade,
            IHttpPaymentFacadeV2 httpPaymentFacadeV2,
            ILogger<PaymentsService> logger)
        {
            _httpPaymentFacade = httpPaymentFacade ?? throw new ArgumentNullException(nameof(httpPaymentFacade));
            _httpPaymentFacadeV2 = httpPaymentFacadeV2 ?? throw new ArgumentNullException(nameof(httpPaymentFacadeV2));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CompletePaymentViewModel> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken)
        {
            try
            {
                var completePaymentResponseDto = await _httpPaymentFacade.CompletePaymentAsync(externalPaymentId, cancellationToken);
                if (string.IsNullOrEmpty(completePaymentResponseDto.Reference))
                {
                    throw new ServiceException(ExceptionMessages.PaymentDataNotFound);
                }
                var viewModel = _mapper.Map<CompletePaymentViewModel>(completePaymentResponseDto);
                return viewModel;
            }
            catch(ServiceException ex)
            {
                _logger.LogError(ex, ExceptionMessages.PaymentDataNotFound);
                throw new ServiceException(ExceptionMessages.PaymentDataNotFound, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionMessages.ErrorRetrievingCompletePayment);
                throw new ServiceException(ExceptionMessages.ErrorRetrievingCompletePayment, ex);
            }
        }

        public async Task<string> InitiatePaymentAsync(PaymentRequestDto? request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.RequestorType) || string.Equals(request.RequestorType, "NA", StringComparison.OrdinalIgnoreCase))
                {
                    return await _httpPaymentFacade.InitiatePaymentAsync(request, cancellationToken);
                }

                return await _httpPaymentFacadeV2.InitiatePaymentAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionMessages.ErrorInitiatePayment);
                throw new ServiceException(ExceptionMessages.ErrorInitiatePayment, ex);
            }
        }
    }
}
