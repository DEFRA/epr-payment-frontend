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
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen] CompletePaymentResponseDto completePaymentResponseDto,
            [Frozen] CompletePaymentViewModel completePaymentViewModel,
            [Greedy] PaymentsService paymentsService,
            Guid externalPaymentId)
        {
            // Arrange

            httpPaymentFacadeMock
                .Setup(facade => facade.CompletePaymentAsync(externalPaymentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(completePaymentResponseDto);

            mapperMock
                .Setup(mapper => mapper.Map<CompletePaymentViewModel>(completePaymentResponseDto))
                .Returns(completePaymentViewModel);

            // Act
            var result = await paymentsService.CompletePaymentAsync(externalPaymentId, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(completePaymentViewModel);
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_WhenReferenceIsNull_ShouldThrowServiceException(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen] CompletePaymentResponseDto completePaymentResponseDto,
            [Frozen(Matching.ImplementedInterfaces)] TestLogger<PaymentsService> testLogger,
            [Frozen] PaymentsService paymentsService,
            Guid externalPaymentId)
        {
            // Arrange
            completePaymentResponseDto.Reference = null;

            httpPaymentFacadeMock
                .Setup(facade => facade.CompletePaymentAsync(externalPaymentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(completePaymentResponseDto);

            // Act
            Func<Task> action = async () => await paymentsService.CompletePaymentAsync(externalPaymentId, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                .WithMessage(ExceptionMessages.PaymentDataNotFound);

                testLogger.LogEntries.Should().ContainSingle()
                                .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.PaymentDataNotFound));
            }
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_WhenExceptionThrown_ShouldLogErrorAndThrowServiceException(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen(Matching.ImplementedInterfaces)] TestLogger<PaymentsService> testLogger,
            [Greedy] PaymentsService paymentsService,
            Guid externalPaymentId)
        {
            // Arrange
            httpPaymentFacadeMock
                .Setup(facade => facade.CompletePaymentAsync(externalPaymentId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            Func<Task> action = async () => await paymentsService.CompletePaymentAsync(externalPaymentId, CancellationToken.None);

            // Assert
            using(new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                    .WithMessage(ExceptionMessages.ErrorRetrievingCompletePayment);

                testLogger.LogEntries.Should().ContainSingle()
                                .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorRetrievingCompletePayment));
            }

        }

        [TestMethod,AutoMoqData]
        public async Task InitiatePaymentAsync_WhenRequestIsValidWithoutRequestorType_ShouldReturnResponseContentFromV1Facade(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen] Mock<IHttpPaymentFacadeV2> httpPaymentFacadeV2Mock,
            [Frozen] TestLogger<PaymentsService> testLogger,
            [Greedy] PaymentsService paymentsService,
            PaymentRequestDto paymentRequestDto,
            string responseContent)
        {
            // Arrange
            paymentRequestDto.RequestorType = null; 

            httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseContent);

            httpPaymentFacadeV2Mock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()));

            // Act
            var result = await paymentsService.InitiatePaymentAsync(paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(responseContent);

                httpPaymentFacadeMock
                    .Verify(facade => facade.InitiatePaymentAsync(It.Is<PaymentRequestDto>(x => x == paymentRequestDto), It.IsAny<CancellationToken>()), Times.Once);

                httpPaymentFacadeV2Mock
                    .Verify(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);

            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_WhenRequestIsValidWithNARequestorType_ShouldReturnResponseContentFromV1Facade(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen] Mock<IHttpPaymentFacadeV2> httpPaymentFacadeV2Mock,
            [Frozen] TestLogger<PaymentsService> testLogger,
            [Greedy] PaymentsService paymentsService,
            PaymentRequestDto paymentRequestDto,
            string responseContent)
        {
            // Arrange
            paymentRequestDto.RequestorType = "NA";

            httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseContent);

            httpPaymentFacadeV2Mock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()));

            // Act
            var result = await paymentsService.InitiatePaymentAsync(paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(responseContent);

                httpPaymentFacadeMock
                    .Verify(facade => facade.InitiatePaymentAsync(It.Is<PaymentRequestDto>(x => x == paymentRequestDto), It.IsAny<CancellationToken>()), Times.Once);

                httpPaymentFacadeV2Mock
                    .Verify(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);

            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_WhenRequestIsValidWithValidRequestorType_ShouldReturnResponseContentFromV2Facade(
                [Frozen] Mock<IMapper> mapperMock,
                [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
                [Frozen] Mock<IHttpPaymentFacadeV2> httpPaymentFacadeV2Mock,
                [Frozen] TestLogger<PaymentsService> testLogger,
                [Greedy] PaymentsService paymentsService,
                PaymentRequestDto paymentRequestDto,
                string responseContent)
        {
            // Arrange
            paymentRequestDto.RequestorType = "Producers";

            httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()));

            httpPaymentFacadeV2Mock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseContent);

            // Act
            var result = await paymentsService.InitiatePaymentAsync(paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(responseContent);

                httpPaymentFacadeMock
                    .Verify(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);

                httpPaymentFacadeV2Mock
                    .Verify(facade => facade.InitiatePaymentAsync(It.Is<PaymentRequestDto>(x => x == paymentRequestDto), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_WhenExceptionThrownFromV1Facade_ShouldLogErrorAndThrowServiceException(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen] Mock<IHttpPaymentFacadeV2> httpPaymentFacadeV2Mock,
            [Frozen(Matching.ImplementedInterfaces)] TestLogger<PaymentsService> testLogger,
            [Greedy] PaymentsService paymentsService,
            PaymentRequestDto paymentRequestDto,
            string responseContent)
        {
            // Arrange
            paymentRequestDto.RequestorType = null;

            httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test Exception"));

            httpPaymentFacadeV2Mock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()));

            // Act
            Func<Task> action = async () => await paymentsService.InitiatePaymentAsync(paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                .WithMessage(ExceptionMessages.ErrorInitiatePayment);

                testLogger.LogEntries.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorInitiatePayment));

                httpPaymentFacadeMock
                    .Verify(facade => facade.InitiatePaymentAsync(It.Is<PaymentRequestDto>(x => x == paymentRequestDto), It.IsAny<CancellationToken>()), Times.Once);

                httpPaymentFacadeV2Mock
                    .Verify(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);
            }

        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_WhenExceptionThrownFromV2Facade_ShouldLogErrorAndThrowServiceException(
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<IHttpPaymentFacade> httpPaymentFacadeMock,
            [Frozen] Mock<IHttpPaymentFacadeV2> httpPaymentFacadeV2Mock,
            [Frozen(Matching.ImplementedInterfaces)] TestLogger<PaymentsService> testLogger,
            [Greedy] PaymentsService paymentsService,
            PaymentRequestDto paymentRequestDto,
            string responseContent)
        {
            // Arrange
            paymentRequestDto.RequestorType = "Exporters";

            httpPaymentFacadeMock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()));

            httpPaymentFacadeV2Mock
                .Setup(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            Func<Task> action = async () => await paymentsService.InitiatePaymentAsync(paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await action.Should().ThrowAsync<ServiceException>()
                .WithMessage(ExceptionMessages.ErrorInitiatePayment);

                testLogger.LogEntries.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorInitiatePayment));

                httpPaymentFacadeMock
                    .Verify(facade => facade.InitiatePaymentAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);

                httpPaymentFacadeV2Mock
                    .Verify(facade => facade.InitiatePaymentAsync(It.Is<PaymentRequestDto>(x => x == paymentRequestDto), It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
