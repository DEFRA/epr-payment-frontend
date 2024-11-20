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
using Microsoft.Identity.Web;
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
                httpContextAccessorMock.Object,
                httpClientFactoryMock.Object,
                Mock.Of<Microsoft.Identity.Web.ITokenAcquisition>(),
                _config,
                featureManagerMock.Object);

            // Act & Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IHttpPaymentFacadeHealthCheckService>();
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenHttpContextAccessorIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock,
            [Frozen] Mock<Microsoft.FeatureManagement.IFeatureManager> featureManagerMock)
        {
            // Arrange
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                null!,
                httpClientFactoryMock.Object,
                Mock.Of<Microsoft.Identity.Web.ITokenAcquisition>(),
                _config,
                featureManagerMock.Object);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("httpContextAccessor");
        }

        [TestMethod]
        public void Constructor_WhenHttpClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var featureManagerMock = new Mock<IFeatureManager>();

            // Act
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                httpContextAccessorMock.Object,
                null!, // Passing null for httpClientFactory
                Mock.Of<ITokenAcquisition>(),
                Mock.Of<IOptions<FacadeService>>(),
                featureManagerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*factory*");
        }


        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock,
            [Frozen] Mock<Microsoft.FeatureManagement.IFeatureManager> featureManagerMock)
        {
            // Arrange
            _config = null!;

            Action act = () => new HttpPaymentFacadeHealthCheckService(
                httpContextAccessorMock.Object,
                httpClientFactoryMock.Object,
                Mock.Of<Microsoft.Identity.Web.ITokenAcquisition>(),
                _config,
                featureManagerMock.Object);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("config");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenUrlInConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock,
            [Frozen] Mock<Microsoft.FeatureManagement.IFeatureManager> featureManagerMock)
        {
            // Arrange
            _config = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = null,
                EndPointName = "health"
            });

            // Act & Assert
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                httpContextAccessorMock.Object,
                httpClientFactoryMock.Object,
                Mock.Of<Microsoft.Identity.Web.ITokenAcquisition>(),
                _config,
                featureManagerMock.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*PaymentFacadeHealthCheck BaseUrl configuration is missing*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenEndPointNameInConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> httpClientFactoryMock,
            [Frozen] Mock<Microsoft.FeatureManagement.IFeatureManager> featureManagerMock)
        {
            // Arrange
            _config = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = "https://example.com",
                EndPointName = null
            });

            // Act & Assert
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                httpContextAccessorMock.Object,
                httpClientFactoryMock.Object,
                Mock.Of<Microsoft.Identity.Web.ITokenAcquisition>(),
                _config,
                featureManagerMock.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*PaymentFacadeHealthCheck EndPointName configuration is missing*");
        }

        [TestMethod]
        public async Task GetHealthAsync_WithCorrectParameters_ShouldCallGet()
        {
            // Arrange
            var baseUrl = "https://example.com";
            var endpointName = "health-check";
            var fullUrl = $"{baseUrl}/{endpointName}/"; // Ensure trailing slash

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var featureManagerMock = new Mock<IFeatureManager>();
            var tokenAcquisitionMock = new Mock<ITokenAcquisition>();
            var configMock = new Mock<IOptions<FacadeService>>();

            configMock.Setup(c => c.Value).Returns(new FacadeService
            {
                Url = baseUrl,
                EndPointName = endpointName,
                DownstreamScope = "scope"
            });

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == fullUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"message\": \"Health check successful\"}", Encoding.UTF8, "application/json")
                });


            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            featureManagerMock
                .Setup(f => f.IsEnabledAsync("EnableAuthenticationFeature"))
                .ReturnsAsync(false); // Authentication disabled

            var service = new HttpPaymentFacadeHealthCheckService(
                httpContextAccessorMock.Object,
                httpClientFactoryMock.Object,
                tokenAcquisitionMock.Object,
                configMock.Object,
                featureManagerMock.Object);

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
                        req.RequestUri!.ToString() == fullUrl),
                    ItExpr.IsAny<CancellationToken>());
            }
        }
    }
}
