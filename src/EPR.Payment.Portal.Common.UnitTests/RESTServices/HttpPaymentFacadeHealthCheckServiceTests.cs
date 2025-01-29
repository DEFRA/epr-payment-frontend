using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace EPR.Payment.Portal.Common.UnitTests.RESTServices
{
    [TestClass]
    public class HttpPaymentFacadeHealthCheckServiceTests
    {
        private IOptions<FacadeService> _config = null!;

        [TestInitialize]
        public void Setup()
        {
            // Configure the options with valid values
            _config = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = "https://example.com",
                EndPointName = "health",
                DownstreamScope = "scope"
            });
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldInitialize_Instance(
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock,
            [Frozen] Mock<Microsoft.FeatureManagement.IFeatureManager> featureManagerMock)
        {
            // Arrange
            var service = new HttpPaymentFacadeHealthCheckService(
                httpClientFactoryMock.Object,
                _config);

            // Act & Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IHttpPaymentFacadeHealthCheckService>();
        }

        [TestMethod]
        public void Constructor_WhenHttpClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var featureManagerMock = new Mock<IFeatureManager>();

            // Act
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                null!,
                _config);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*factory*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock)
        {
            // Arrange
            IOptions<FacadeService>? nullConfig = null;

            Action act = () => new HttpPaymentFacadeHealthCheckService(
                httpClientFactoryMock.Object,
                nullConfig);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("config");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenUrlInConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock)
        {
            // Arrange
            var invalidConfig = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = null
            });

            // Act & Assert
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                httpClientFactoryMock.Object,
                invalidConfig);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*PaymentFacadeHealthCheck BaseUrl configuration is missing*");
        }

        [TestMethod]
        public async Task GetHealthAsync_WithCorrectParameters_ShouldCallGet()
        {
            // Arrange
            var baseUrl = "https://example.com";
            var expectedUrl = $"{baseUrl}/admin/health";

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var configMock = new Mock<IOptions<FacadeService>>();

            configMock.Setup(c => c.Value).Returns(new FacadeService
            {
                Url = baseUrl
            });

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"message\": \"Health check successful\"}", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new HttpPaymentFacadeHealthCheckService(
                httpClientFactoryMock.Object,
                configMock.Object);

            // Act
            HttpResponseMessage? result = await service.GetHealthAsync(CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.StatusCode.Should().Be(HttpStatusCode.OK);

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

    }
}
