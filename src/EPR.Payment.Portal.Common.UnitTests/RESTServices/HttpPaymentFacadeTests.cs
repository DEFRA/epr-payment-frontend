using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace EPR.Payment.Portal.Common.UnitTests.RESTServices
{
    [TestClass]
    public class HttpPaymentFacadeTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
        private Mock<IOptions<FacadeService>> _configMock = null!;


        [TestInitialize]
        public void Initialize()
        {
            // Mock configuration
            var config = new FacadeService
            {
                Url = "https://example.com",
                EndPointName = "payments",
                HttpClientName = "HttpClientName"
            };

            _configMock = new Mock<IOptions<FacadeService>>();
            _configMock.Setup(x => x.Value).Returns(config);

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        private HttpPaymentFacade CreateHttpPaymentsService(HttpClient httpClient)
        {
            return new HttpPaymentFacade(
                _httpContextAccessorMock!.Object,
                new HttpClientFactoryMock(httpClient),
                _configMock!.Object);
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_Success_ReturnsPaymentDetailsDto(
            [Frozen] Mock<HttpMessageHandler> handlerMock,
            HttpPaymentFacade httpPaymentsFacade,
            Guid externalPaymentId,
            CompletePaymentResponseDto completePaymentResponseDto, CancellationToken cancellationToken)
        {
            // Arrange
            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ReturnsAsync(new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content = new StringContent(JsonConvert.SerializeObject(completePaymentResponseDto), Encoding.UTF8, "application/json")
                       });

            var httpClient = new HttpClient(handlerMock.Object);
            httpPaymentsFacade = CreateHttpPaymentsService(httpClient);

            // Act
            var result = await httpPaymentsFacade.CompletePaymentAsync(externalPaymentId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(completePaymentResponseDto);
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_Failure_ThrowsException(
            [Frozen] Mock<HttpMessageHandler> handlerMock,
            HttpPaymentFacade httpPaymentsFacade,
            Guid externalPaymentId, CancellationToken cancellationToken)
        {
            // Arrange

            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ThrowsAsync(new HttpRequestException(ExceptionMessages.ErrorCompletePayment));

            var httpClient = new HttpClient(handlerMock.Object);
            httpPaymentsFacade = CreateHttpPaymentsService(httpClient);

            // Act
            Func<Task> act = async () => await httpPaymentsFacade.CompletePaymentAsync(externalPaymentId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>().WithMessage(ExceptionMessages.ErrorCompletePayment);
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_Success_ReturnsPaymentDetailsDto(
            [Frozen] Mock<HttpMessageHandler> handlerMock,
            HttpPaymentFacade httpPaymentsFacade,
            PaymentRequestDto request,
            string response, CancellationToken cancellationToken)
        {
            // Arrange
            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ReturnsAsync(new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
                       });

            var httpClient = new HttpClient(handlerMock.Object);
            httpPaymentsFacade = CreateHttpPaymentsService(httpClient);

            // Act
            var result = await httpPaymentsFacade.InitiatePaymentAsync(request, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(response);
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePaymentAsync_Failure_ThrowsException(
            [Frozen] Mock<HttpMessageHandler> handlerMock,
            HttpPaymentFacade httpPaymentsFacade,
            PaymentRequestDto request, CancellationToken cancellationToken)
        {
            // Arrange

            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                       .ThrowsAsync(new HttpRequestException(ExceptionMessages.ErrorCompletePayment));

            var httpClient = new HttpClient(handlerMock.Object);
            httpPaymentsFacade = CreateHttpPaymentsService(httpClient);

            // Act
            Func<Task> act = async () => await httpPaymentsFacade.InitiatePaymentAsync(request, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>().WithMessage(ExceptionMessages.ErrorInitiatePayment);
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>());
            }
        }
    }
}
