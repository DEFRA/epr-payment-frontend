using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Controllers.Culture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class CultureControllerTests
    {
        private const string ReturnUrl = "returnUrl";
        private const string CultureEn = "en";

        private Mock<IResponseCookies> _responseCookiesMock = null!;
        private Mock<ISession> _sessionMock = null!;
        private Mock<HttpContext> _httpContextMock = null!;
        private CultureController _systemUnderTest = null!;
        private readonly TestLogger<CultureController> _testLogger = new TestLogger<CultureController>();

        [TestInitialize]
        public void Setup()
        {
            _responseCookiesMock = new Mock<IResponseCookies>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();

            // Set up the controller with mock HttpContext
            _systemUnderTest = new CultureController (_testLogger)
            {
                ControllerContext = { HttpContext = _httpContextMock.Object }
            };

            _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);
        }

        [TestMethod]
        public void CultureController_UpdateCulture_RedirectsToReturnUrlWithCulture()
        {
            // Arrange
            _httpContextMock
                .Setup(x => x.Response.Cookies)
                .Returns(_responseCookiesMock.Object);

            var cultureBytes = Encoding.UTF8.GetBytes(CultureEn);

            // Act
            var result = _systemUnderTest.UpdateCulture(CultureEn, ReturnUrl);

            // Assert
            using (new AssertionScope())
            {
                result.Url.Should().Be(ReturnUrl);
                _sessionMock.Verify(x => x.Set(Language.SessionLanguageKey, cultureBytes), Times.Once);
            }
        }

        [TestMethod]
        public void CultureController_UpdateCulture_LogsErrorAndRedirectsOnException()
        {
            // Arrange
            var exceptionMessage = "Session error";
            _sessionMock
                .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _systemUnderTest.UpdateCulture(CultureEn, ReturnUrl);

            // Assert
            using (new AssertionScope())
            {
                result.Url.Should().Be(ReturnUrl); // Verify redirect URL

                _testLogger.LogEntries[0].Should().BeEquivalentTo((LogLevel.Error, "Inside Updateculture"));

                _testLogger.LogEntries[1].Should().BeEquivalentTo((LogLevel.Error, "UpdateCulture Error Message: Session error"));
            }
        }
    }
}
