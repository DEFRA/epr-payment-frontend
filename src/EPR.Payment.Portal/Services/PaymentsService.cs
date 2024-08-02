using AutoMapper;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IHttpPaymentFacade _httpPaymentFacade;
        private readonly ILogger<PaymentsService> _logger;
        private readonly IMapper _mapper;

        public PaymentsService(IMapper mapper, IHttpPaymentFacade httpPaymentFacade, ILogger<PaymentsService> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpPaymentFacade = httpPaymentFacade ?? throw new ArgumentNullException(nameof(httpPaymentFacade));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<CompletePaymentViewModel> CompletePaymentAsync(Guid externalPaymentId, CancellationToken cancellationToken)
        {
            if (externalPaymentId == Guid.Empty)
            {
                _logger.LogError(ExceptionMessages.ErrorExternalPaymentIdEmpty);
                throw new ArgumentException(ExceptionMessages.ErrorExternalPaymentIdEmpty);
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
                _logger.LogError(ex, ExceptionMessages.ErrorRetrievingCompletePayment);
                throw new Exception(ExceptionMessages.ErrorRetrievingCompletePayment, ex);
            }
        }

        public async Task<string> InitiatePaymentAsync(PaymentRequestDto? request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError(ExceptionMessages.ErrorInvalidPaymentRequestDto);
                throw new ArgumentException(ExceptionMessages.ErrorInvalidPaymentRequestDto);
            }
            try
            {
                return await _httpPaymentFacade.InitiatePaymentAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionMessages.ErrorInitiatePayment);
                throw new Exception(ExceptionMessages.ErrorInitiatePayment, ex);
            }
        }
    }
}
