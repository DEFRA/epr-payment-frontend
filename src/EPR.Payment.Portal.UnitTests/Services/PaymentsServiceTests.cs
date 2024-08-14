using AutoFixture.MSTest;
using AutoMapper;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Exceptions;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class PaymentsServiceTests
    {
        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_WhenResponseIsValid_ShouldReturnMappedViewModel(
            [Frozen] Mock<IMapper> _mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> _httpPaymentFacadeMock,
            [Frozen] Mock<ILogger<PaymentsService>> _loggerMock,
            [Frozen] PaymentsService _paymentsService,
            [Frozen] Guid _externalPaymentId,
            [Frozen] CompletePaymentResponseDto _completePaymentResponseDto,
            [Frozen] CompletePaymentViewModel _completePaymentViewModel)
        {
            // Arrange
            _paymentsService = new PaymentsService(_mapperMock.Object, _httpPaymentFacadeMock.Object, _loggerMock.Object);

            _httpPaymentFacadeMock
                .Setup(facade => facade.CompletePaymentAsync(_externalPaymentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_completePaymentResponseDto);

            _mapperMock
                .Setup(mapper => mapper.Map<CompletePaymentViewModel>(_completePaymentResponseDto))
                .Returns(_completePaymentViewModel);

            // Act
            var result = await _paymentsService.CompletePaymentAsync(_externalPaymentId, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(_completePaymentViewModel);
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_WhenReferenceIsNull_ShouldThrowServiceException(
            [Frozen] Mock<IMapper> _mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> _httpPaymentFacadeMock,
            [Frozen] PaymentsService _paymentsService,
            [Frozen] Guid _externalPaymentId,
            [Frozen] CompletePaymentResponseDto _completePaymentResponseDto,
            [Frozen] TestLogger<PaymentsService> _testLogger)
        {
            // Arrange
            _paymentsService = new PaymentsService(_mapperMock.Object, _httpPaymentFacadeMock.Object, _testLogger);
            _completePaymentResponseDto.Reference = null;

            _httpPaymentFacadeMock
                .Setup(facade => facade.CompletePaymentAsync(_externalPaymentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_completePaymentResponseDto);

            // Act
            Func<Task> action = async () => await _paymentsService.CompletePaymentAsync(_externalPaymentId, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                .WithMessage(ExceptionMessages.PaymentDataNotFound);

                _testLogger.LogEntries.Should().ContainSingle()
                                .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.PaymentDataNotFound));
            }
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_WhenExceptionThrown_ShouldLogErrorAndThrowServiceException(
            [Frozen] Mock<IMapper> _mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> _httpPaymentFacadeMock,
            [Frozen] PaymentsService _paymentsService,
            [Frozen] Guid _externalPaymentId,
            [Frozen] TestLogger<PaymentsService> _testLogger)
        {
            // Arrange
            _paymentsService = new PaymentsService(_mapperMock.Object, _httpPaymentFacadeMock.Object, _testLogger);
            _httpPaymentFacadeMock
                .Setup(facade => facade.CompletePaymentAsync(_externalPaymentId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            Func<Task> action = async () => await _paymentsService.CompletePaymentAsync(_externalPaymentId, CancellationToken.None);

            // Assert
            using(new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                    .WithMessage(ExceptionMessages.ErrorRetrievingCompletePayment);

                _testLogger.LogEntries.Should().ContainSingle()
                                .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorRetrievingCompletePayment));
            }

        }

        [TestMethod,AutoMoqData]
        public async Task InitiatePaymentAsync_WhenRequestIsValid_ShouldReturnResponseContent(
            [Frozen] Mock<IMapper> _mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> _httpPaymentFacadeMock,
            [Frozen] PaymentsService _paymentsService,
            [Frozen] TestLogger<PaymentsService> _testLogger,
            [Frozen] PaymentRequestDto _paymentRequestDto,
            [Frozen] string _responseContent)
        {
            // Arrange
            _paymentsService = new PaymentsService(_mapperMock.Object, _httpPaymentFacadeMock.Object, _testLogger);

            _httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(_paymentRequestDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_responseContent);

            // Act
            var result = await _paymentsService.InitiatePaymentAsync(_paymentRequestDto, CancellationToken.None);

            // Assert
            result.Should().Be(_responseContent);
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_WhenExceptionThrown_ShouldLogErrorAndThrowServiceException(
            [Frozen] Mock<IMapper> _mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> _httpPaymentFacadeMock,
            [Frozen] PaymentsService _paymentsService,
            [Frozen] TestLogger<PaymentsService> _testLogger,
            [Frozen] PaymentRequestDto _paymentRequestDto,
            [Frozen] string _responseContent)
        {
            // Arrange
            _paymentsService = new PaymentsService(_mapperMock.Object, _httpPaymentFacadeMock.Object, _testLogger);
            _httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(_paymentRequestDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            Func<Task> action = async () => await _paymentsService.InitiatePaymentAsync(_paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                .WithMessage(ExceptionMessages.ErrorInitiatePayment);

                _testLogger.LogEntries.Should().ContainSingle()
                                .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorInitiatePayment));
            }
        }
    }
}
