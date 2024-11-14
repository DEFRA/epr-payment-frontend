using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;

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
                EndPointName = "health"
            });
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldInitialize_Instance(
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            var service = new HttpPaymentFacadeHealthCheckService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                _config);

            // Act & Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IHttpPaymentFacadeHealthCheckService>();
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenHttpContextAccessorIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                null!,
                _httpClientFactoryMock.Object,
                _config);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("httpContextAccessor");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenHttpClientFactoryIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                _httpContextAccessorMock.Object,
                null!,
                _config);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("httpClientFactory");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            _config = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = null,
                EndPointName = null
            });

            Action act = () => new HttpPaymentFacadeHealthCheckService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                _config);

            // Act & Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("config");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenUrlInConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            _config = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = null,
                EndPointName = "health"
            });

            // Act & Assert
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                _config);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*PaymentFacadeHealthCheck BaseUrl configuration is missing*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenEndPointNameInConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            _config = Microsoft.Extensions.Options.Options.Create(new FacadeService
            {
                Url = "https://example.com",
                EndPointName = null
            });

            // Act & Assert
            Action act = () => new HttpPaymentFacadeHealthCheckService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                _config);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*PaymentFacadeHealthCheck EndPointName configuration is missing*");
        }

        [TestMethod, AutoMoqData]
        public async Task GetHealthAsync_WithCorrectParameters_ShouldCallGet(
            [Frozen] Mock<HttpMessageHandler> _handlerMock,
            [Frozen] Mock<IHttpContextAccessor> _httpContextAccessorMock,
            [Frozen] Mock<IHttpClientFactory> _httpClientFactoryMock)
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            string healthCheckJson = @"
                {
                  ""status"": ""Healthy"",
                  ""results"": {
                    ""AppDbContext"": {
                      ""status"": ""Healthy"",
                      ""description"": null,
                      ""data"": {}
                    },
                    ""Payment Status Health Check"": {
                      ""status"": ""Healthy"",
                      ""description"": ""Payment Status Health Check"",
                      ""data"": {}
                    }
                  }
                }";

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(healthCheckJson)
            };

            var baseHttpServiceMock = new Mock<BaseHttpService>(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                _config);

            _handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                       ItExpr.IsAny<CancellationToken>())
                       .ReturnsAsync(httpResponseMessage)
                       .Verifiable();

            var httpClient = new HttpClient(_handlerMock.Object);

            var service = new HttpPaymentFacadeHealthCheckService(
                _httpContextAccessorMock.Object,
                new HttpClientFactoryMock(httpClient),
                _config);

            // Act
            var result = await service.GetHealthAsync(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                baseHttpServiceMock.Verify();
            }

        }

    }
}
