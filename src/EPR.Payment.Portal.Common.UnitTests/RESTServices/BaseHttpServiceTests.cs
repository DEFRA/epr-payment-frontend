using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.Payment.Portal.Common.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System.Net;

namespace EPR.Payment.Portal.Common.UnitTests.RESTServices
{
    [TestClass]
    public class BaseHttpServiceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
        private Mock<IHttpClientFactory> _httpClientFactoryMock = null!;
        private Mock<ITokenAcquisition> _tokenAcquisitionMock = null!;
        private Mock<IFeatureManager> _featureManagerMock = null!;
        private Mock<HttpMessageHandler> _handlerMock = null!;
        private HttpClient _httpClient = null!;
        private TestableBaseHttpService _testableHttpService = null!;
        private IFixture _fixture = null!;
        private const string baseUrl = "http://paymentfacadedummy.com";
        private const string endPointName = "api";
        private const string url = "paymentfacadeurl";
        private string expectedUrl = string.Empty;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _httpContextAccessorMock = _fixture.Freeze<Mock<IHttpContextAccessor>>();
            _httpClientFactoryMock = _fixture.Freeze<Mock<IHttpClientFactory>>();
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _httpClient = new HttpClient(_handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            _tokenAcquisitionMock = new Mock<ITokenAcquisition>();
            _featureManagerMock = new Mock<IFeatureManager>();

            _featureManagerMock
                .Setup(f => f.IsEnabledAsync("EnableAuthenticationFeature"))
                .ReturnsAsync(true); // Ensure authentication is enabled

            _tokenAcquisitionMock
                .Setup(t => t.GetAccessTokenForUserAsync(
                    It.IsAny<IEnumerable<string>>(),
                    null, // Replace with actual tenantId if used
                    null, // Replace with ClaimsPrincipal if used
                    null, // Replace with user flow if used
                    It.IsAny<TokenAcquisitionOptions?>()))
                .ReturnsAsync("test-access-token"); // Simulate token acquisition

            expectedUrl = $"{baseUrl}/{endPointName}/{url}/";

            _testableHttpService = new TestableBaseHttpService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                baseUrl,
                endPointName,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);
        }



        [TestMethod]
        public async Task Get_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            var result = await _testableHttpService.PublicGet<object>(url, cancellationToken);
            // Parse the result to JToken and assert the expected JSON token
            var jsonResult = JToken.Parse(result!.ToString()!);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                jsonResult["result"]!.ToString().Should().Be("success");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Get &&
                        msg.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task Post_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            var result = await _testableHttpService.PublicPost<object>(url, payload, cancellationToken);
            // Parse the result to JToken and assert the expected JSON token
            var jsonResult = JToken.Parse(result!.ToString()!);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                jsonResult["result"]!.ToString().Should().Be("success");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task PostNonGeneric_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            await _testableHttpService.PublicPost(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task Put_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            var result = await _testableHttpService.PublicPut<object>(url, payload, cancellationToken);
            // Parse the result to JToken and assert the expected JSON token
            var jsonResult = JToken.Parse(result!.ToString()!);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                jsonResult["result"]!.ToString().Should().Be("success");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Put &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task PutNonGeneric_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            await _testableHttpService.PublicPut(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Put &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task Delete_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            var result = await _testableHttpService.PublicDelete<object>(url, payload, cancellationToken);
            // Parse the result to JToken and assert the expected JSON token
            var jsonResult = JToken.Parse(result!.ToString()!);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                jsonResult["result"]!.ToString().Should().Be("success");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Delete &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task DeleteNonGeneric_ShouldCallSendWithCorrectParametersAndReturnResult()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;
            var responseContent = "{\"result\": \"success\"}";

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            await _testableHttpService.PublicDelete(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Delete &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }

        }

        [TestMethod]
        public async Task Get_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicGet<object>(url, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ResponseCodeException>()
                    .WithMessage("*Bad Request*");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Get &&
                        msg.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod]
        public async Task Get_WhenUrlIsEmpty_ShouldThrowException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicGet<object>("", cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>();

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Never(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Get &&
                        msg.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod]
        public async Task Post_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPost<object>(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ResponseCodeException>()
                    .WithMessage("*Bad Request*");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod]
        public async Task Post_WhenUrlIsEmpty_ShouldThrowArgumentNullException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPost<object>("", payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>()
                    .WithMessage("*URL cannot be null or empty.*");
            }
        }

        [TestMethod]
        public async Task PostNonGeneric_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPost(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("Error occurred calling API with error code: BadRequest. Message: Bad Request");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Post &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod]
        public async Task PostNonGeneric_WhenUrlIsEmpty_ShouldThrowArgumentNullException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPost("", payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>()
                    .WithMessage("*Value cannot be null or empty.*");
            }
        }

        [TestMethod]
        public async Task Put_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPut<object>(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ResponseCodeException>()
                    .WithMessage("*Bad Request*");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Put &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod]
        public async Task Put_WhenUrlIsEmpty_ShouldThrowArgumentNullException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPut<object>("", payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>()
                    .WithMessage("*Value cannot be null or empty*");
            }
        }

        [TestMethod]
        public async Task PutNonGeneric_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPut(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("Error occurred calling API with error code: BadRequest. Message: Bad Request");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Put &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }
        }


        [TestMethod]
        public async Task Put_NonGenericWhenUrlIsEmpty_ShouldThrowArgumentNullException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPut("", payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>()
                    .WithMessage("*Value cannot be null or empty*");
            }
        }



        [TestMethod]
        public async Task Delete_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicDelete<object>(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ResponseCodeException>()
                    .WithMessage("*Bad Request*");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Delete &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }
        }

        [TestMethod]
        public async Task Delete_WhenUrlIsEmpty_ShouldThrowArgumentNullException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicDelete<object>("", payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>()
                    .WithMessage("*Value cannot be null*");
            }
        }

        [TestMethod]
        public async Task DeleteNonGeneric_WhenResponseIsUnsuccessful_ShouldThrowResponseCodeException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicDelete(url, payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("Error occurred calling API with error code: BadRequest. Message: Bad Request");

                _handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(msg =>
                        msg.Method == HttpMethod.Delete &&
                        msg.RequestUri!.ToString() == expectedUrl &&
                        msg.Content!.Headers.ContentType!.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>());
            }
        }


        [TestMethod]
        public async Task DeleteNonGeneric_WhenUrlIsEmpty_ShouldThrowArgumentNullException()
        {
            // Arrange
            var payload = new { Id = 1, Name = "Test" };
            var cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicDelete("", payload, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>()
                    .WithMessage("*URL cannot be null or empty.*");
            }
        }

        [TestMethod]
        public void SetBearerToken_ShouldSetAuthorizationHeader()
        {
            // Arrange
            var token = "test-token";

            // Act
            _testableHttpService.PublicSetBearerToken(token);

            // Assert
            using (new AssertionScope())
            {
                _httpClient.DefaultRequestHeaders.Authorization.Should().NotBeNull();
                _httpClient.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
                _httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);
            }
        }


        [TestMethod]
        public void Constructor_WhenHttpContextAccessorIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new TestableBaseHttpService(
                null!,
                _httpClientFactoryMock.Object,
                baseUrl,
                endPointName,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*httpContextAccessor*");
        }

        [TestMethod]
        public void Constructor_WhenHttpClientFactoryIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new TestableBaseHttpService(
                _httpContextAccessorMock.Object,
                null!, // HttpClientFactory is null
                baseUrl,
                endPointName,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'httpClientFactory')");
        }

        [TestMethod]
        public void Constructor_WhenBaseUrlIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new TestableBaseHttpService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                null!,
                endPointName,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*baseUrl*");
        }

        [TestMethod]
        public void Constructor_WhenEndPointNameIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new TestableBaseHttpService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                baseUrl,
                null!,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*endPointName*");
        }

        [TestMethod]
        public void Constructor_WhenTrailingSlash_ShouldTrimTrailingSlashFromBaseUrl()
        {
            // Arrange
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            // Act
            var service = new TestableBaseHttpService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                $"{baseUrl}/",
                endPointName,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);

            // Assert
            service.BaseUrl.Should().Be($"{baseUrl}/{endPointName}");
        }

        [TestMethod]
        public void Constructor_WhenNoTrailingSlash_ShouldNotTrim()
        {
            // Arrange
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            // Act
            var service = new TestableBaseHttpService(
                _httpContextAccessorMock.Object,
                _httpClientFactoryMock.Object,
                baseUrl,
                endPointName,
                _tokenAcquisitionMock.Object,
                "scope",
                _featureManagerMock.Object);

            // Assert
            service.BaseUrl.Should().Be($"{baseUrl}/{endPointName}");
        }

        [TestMethod]
        public async Task PrepareAuthenticatedClient_ShouldSetAuthorizationHeader()
        {
            // Act
            await _testableHttpService.PublicPrepareAuthenticatedClient();

            // Assert
            _httpClient.DefaultRequestHeaders.Authorization.Should().NotBeNull();
            _httpClient.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
            _httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be("test-access-token");
        }

        [TestMethod]
        public async Task PrepareAuthenticatedClient_WhenFeatureFlagDisabled_ShouldNotSetAuthorizationHeader()
        {
            // Arrange
            _featureManagerMock
                .Setup(f => f.IsEnabledAsync("EnableAuthenticationFeature"))
                .ReturnsAsync(false); // Simulate feature flag being disabled

            // Act
            await _testableHttpService.PublicPrepareAuthenticatedClient();

            // Assert
            using (new AssertionScope())
            {
                _httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task PrepareAuthenticatedClient_WhenFeatureFlagThrowsException_ShouldNotSetAuthorizationHeader()
        {
            // Arrange
            _featureManagerMock
                .Setup(f => f.IsEnabledAsync("EnableAuthenticationFeature"))
                .ThrowsAsync(new Exception("Feature flag error"));

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPrepareAuthenticatedClient();

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("Feature flag error");

                _httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task PrepareAuthenticatedClient_WhenTokenAcquisitionFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _tokenAcquisitionMock
                .Setup(t => t.GetAccessTokenForUserAsync(
                    It.IsAny<IEnumerable<string>>(),
                    null,  // Tenant ID
                    null,  // ClaimsPrincipal
                    null,  // User Flow
                    null   // TokenAcquisitionOptions
                ))
                .ThrowsAsync(new Exception("Token acquisition failed"));

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPrepareAuthenticatedClient();

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("Failed to prepare the authenticated client.");
            }
        }

        [TestMethod]
        public async Task PrepareAuthenticatedClient_WhenTokenAcquisitionReturnsNullOrEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _tokenAcquisitionMock
                .Setup(t => t.GetAccessTokenForUserAsync(
                    It.IsAny<IEnumerable<string>>(),
                    null,  // Tenant ID
                    null,  // ClaimsPrincipal
                    null,  // User Flow
                    null   // TokenAcquisitionOptions
                ))
                .ReturnsAsync(string.Empty); // Simulate token acquisition returning null or empty

            // Act
            Func<Task> act = async () => await _testableHttpService.PublicPrepareAuthenticatedClient();

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("Failed to prepare the authenticated client."); // Match the actual exception message
            }
        }

        [TestMethod]
        public async Task PrepareAuthenticatedClient_WhenFeatureFlagEnabledAndTokenAcquired_ShouldSetAuthorizationHeader()
        {
            // Arrange
            _featureManagerMock
                .Setup(f => f.IsEnabledAsync("EnableAuthenticationFeature"))
                .ReturnsAsync(true);

            _tokenAcquisitionMock
                .Setup(t => t.GetAccessTokenForUserAsync(
                    It.IsAny<IEnumerable<string>>(),
                    null,  // Tenant ID
                    null,  // ClaimsPrincipal
                    null,  // User Flow
                    null   // TokenAcquisitionOptions
                ))
                .ReturnsAsync("test-token");

            // Act
            await _testableHttpService.PublicPrepareAuthenticatedClient();

            // Assert
            using (new AssertionScope())
            {
                _httpClient.DefaultRequestHeaders.Authorization.Should().NotBeNull();
                _httpClient.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
                _httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be("test-token");
            }
        }

        [TestMethod]
        public async Task Get_WhenFeatureFlagDisabled_ShouldNotSetAuthorizationHeader()
        {
            // Arrange
            _featureManagerMock
                .Setup(f => f.IsEnabledAsync("EnableAuthenticationFeature"))
                .ReturnsAsync(false); // Simulate feature flag being disabled

            var responseContent = "{\"result\": \"success\"}";
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            // Act
            var result = await _testableHttpService.PublicGet<object>(url, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                _httpClient.DefaultRequestHeaders.Authorization.Should().BeNull(); // Verify no Authorization header
            }
        }
    }
}