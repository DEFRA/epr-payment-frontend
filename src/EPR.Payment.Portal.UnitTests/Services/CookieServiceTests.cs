using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using CookieOptions = EPR.Payment.Portal.Common.Options.CookieOptions;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class CookieServiceTests
    {
        private Mock<ILogger<CookieService>>? _mockLogger;
        private Mock<IOptions<CookieOptions>>? _mockCookieOptions;
        private Mock<IOptions<GoogleAnalyticsOptions>>? _mockGoogleAnalyticsOptions;
        private CookieService? _service;

        [TestInitialize]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<CookieService>>();
            _mockCookieOptions = new Mock<IOptions<CookieOptions>>();
            _mockGoogleAnalyticsOptions = new Mock<IOptions<GoogleAnalyticsOptions>>();

            _mockCookieOptions.Setup(x => x.Value).Returns(new CookieOptions
            {
                SessionCookieName = "SessionCookie",
                CookiePolicyCookieName = "CookiePolicy",
                CookiePolicyDurationInMonths = 3,
                AntiForgeryCookieName = "AntiForgery",
                TempDataCookie = "TempData",
                TsCookieName = "TsCookie",
                B2CCookieName = "B2CCookie",
                OpenIdCookieName = "OpenIdCookie",
                CorrelationCookieName = "CorrelationCookie",
                AuthenticationCookieName = "AuthenticationCookie"
            });

            _mockGoogleAnalyticsOptions.Setup(x => x.Value).Returns(new GoogleAnalyticsOptions
            {
                CookiePrefix = "_ga",
                MeasurementId = "G-12345678",
                TagManagerContainerId = "GTM-1234567"
            });

            _service = new CookieService(
                _mockLogger.Object,
                _mockCookieOptions.Object,
                _mockGoogleAnalyticsOptions.Object);
        }

        [TestMethod]
        public void SetCookieAcceptance_AcceptTrue_ShouldSetCookie()
        {
            var responseCookiesMock = new Mock<IResponseCookies>();
            var requestCookiesMock = new Mock<IRequestCookieCollection>();

            _service?.SetCookieAcceptance(true, requestCookiesMock.Object, responseCookiesMock.Object);

            responseCookiesMock.Verify(rc => rc.Append(
                "CookiePolicy",
                "True",
                It.Is<Microsoft.AspNetCore.Http.CookieOptions>(options =>
                    options.Expires.HasValue &&
                    options.Expires.Value > DateTimeOffset.UtcNow &&
                    options.HttpOnly &&
                    options.Secure &&
                    options.SameSite == SameSiteMode.Lax)),
                Times.Once);
        }

        [TestMethod]
        public void SetCookieAcceptance_AcceptFalse_ShouldExpireGoogleAnalyticsCookies()
        {
            var responseCookiesMock = new Mock<IResponseCookies>();
            var requestCookiesMock = new Mock<IRequestCookieCollection>();

            var cookies = new[]
            {
                new KeyValuePair<string, string>("_ga_123", "value1"),
                new KeyValuePair<string, string>("_ga_456", "value2")
            };

            requestCookiesMock.Setup(c => c.GetEnumerator()).Returns(cookies.AsEnumerable().GetEnumerator());

            _service?.SetCookieAcceptance(false, requestCookiesMock.Object, responseCookiesMock.Object);

            using (new AssertionScope())
            {
                responseCookiesMock.Verify(rc => rc.Append(
                    "_ga_123",
                    "value1",
                    It.Is<Microsoft.AspNetCore.Http.CookieOptions>(options =>
                        options.Expires.HasValue &&
                        options.Expires.Value < DateTimeOffset.UtcNow &&
                        options.HttpOnly &&
                        options.Secure &&
                        options.SameSite == SameSiteMode.Strict)),
                    Times.Once);

                responseCookiesMock.Verify(rc => rc.Append(
                    "_ga_456",
                    "value2",
                    It.Is<Microsoft.AspNetCore.Http.CookieOptions>(options =>
                        options.Expires.HasValue &&
                        options.Expires.Value < DateTimeOffset.UtcNow &&
                        options.HttpOnly &&
                        options.Secure &&
                        options.SameSite == SameSiteMode.Strict)),
                    Times.Once);

                responseCookiesMock.Verify(rc => rc.Append(
                    "CookiePolicy",
                    "False",
                    It.Is<Microsoft.AspNetCore.Http.CookieOptions>(options =>
                        options.Expires.HasValue &&
                        options.Expires.Value > DateTimeOffset.UtcNow &&
                        options.HttpOnly &&
                        options.Secure &&
                        options.SameSite == SameSiteMode.Lax)),
                    Times.Once);
            }
        }

        [TestMethod]
        public void SetCookieAcceptance_WhenExceptionOccurs_ShouldLogErrorAndThrow()
        {
            var responseCookiesMock = new Mock<IResponseCookies>();
            var requestCookiesMock = new Mock<IRequestCookieCollection>();

            var faultyOptions = new CookieOptions
            {
                CookiePolicyCookieName = null!,
                SessionCookieName = "SessionCookie",
                CookiePolicyDurationInMonths = 3,
                AntiForgeryCookieName = "AntiForgery",
                TempDataCookie = "TempData",
                TsCookieName = "TsCookie",
                B2CCookieName = "B2CCookie",
                OpenIdCookieName = "OpenIdCookie",
                CorrelationCookieName = "CorrelationCookie",
                AuthenticationCookieName = "AuthenticationCookie"
            };

            var faultyService = new CookieService(
                _mockLogger!.Object,
                Microsoft.Extensions.Options.Options.Create(faultyOptions),
                _mockGoogleAnalyticsOptions!.Object);

            Action act = () => faultyService.SetCookieAcceptance(true, requestCookiesMock.Object, responseCookiesMock.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'cookieName')");
            VerifyLog("Error setting cookie acceptance to 'True'", LogLevel.Error);
        }

        [TestMethod]
        public void HasUserAcceptedCookies_ValidCookie_ShouldReturnTrue()
        {
            var requestCookiesMock = new Mock<IRequestCookieCollection>();
            requestCookiesMock.Setup(c => c["CookiePolicy"]).Returns("True");

            var result = _service?.HasUserAcceptedCookies(requestCookiesMock.Object);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void HasUserAcceptedCookies_InvalidCookie_ShouldReturnFalse()
        {
            var requestCookiesMock = new Mock<IRequestCookieCollection>();
            requestCookiesMock.Setup(c => c["CookiePolicy"]).Returns("InvalidValue");

            var result = _service?.HasUserAcceptedCookies(requestCookiesMock.Object);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void HasUserAcceptedCookies_WhenExceptionOccurs_ShouldLogErrorAndThrow()
        {
            var requestCookiesMock = new Mock<IRequestCookieCollection>();

            var faultyOptions = new CookieOptions
            {
                CookiePolicyCookieName = null!,
                SessionCookieName = "SessionCookie",
                CookiePolicyDurationInMonths = 3,
                AntiForgeryCookieName = "AntiForgery",
                TempDataCookie = "TempData",
                TsCookieName = "TsCookie",
                B2CCookieName = "B2CCookie",
                OpenIdCookieName = "OpenIdCookie",
                CorrelationCookieName = "CorrelationCookie",
                AuthenticationCookieName = "AuthenticationCookie"
            };

            var faultyService = new CookieService(
                _mockLogger!.Object,
                Microsoft.Extensions.Options.Options.Create(faultyOptions),
                _mockGoogleAnalyticsOptions!.Object);

            Action act = () => faultyService.HasUserAcceptedCookies(requestCookiesMock.Object);

            using (new AssertionScope())
            {
                act.Should().Throw<ArgumentNullException>()
                    .WithMessage("Value cannot be null. (Parameter 'cookieName')");

                VerifyLog("Error reading cookie acceptance", LogLevel.Error);
            }
        }

        private void VerifyLog(string expectedMessage, LogLevel expectedLogLevel)
        {
            _mockLogger?.Verify(logger =>
                logger.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == expectedMessage),
                    It.IsAny<Exception?>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }
    }
}