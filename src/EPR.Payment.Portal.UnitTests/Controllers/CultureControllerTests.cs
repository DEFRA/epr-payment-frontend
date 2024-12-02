using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Controllers.Culture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using System.Text.RegularExpressions;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class CultureControllerTests
    {
        private const string ReturnUrl = "returnUrl";
        private const string CultureEn = "en";

        private Mock<ISession> _sessionMock = null!;
        private Mock<HttpContext> _httpContextMock = null!;
        private Mock<HttpResponse> _responseMock = null!;
        private CultureController _systemUnderTest = null!;

        private const string CultureKey = Language.SessionLanguageKey;

        [TestInitialize]
        public void Setup()
        {
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _responseMock = new Mock<HttpResponse>();

            // Mock Response.Headers
            var headers = new HeaderDictionary();
            _responseMock.SetupGet(x => x.Headers).Returns(headers);

            // Set up HttpContext
            _httpContextMock.SetupGet(x => x.Session).Returns(_sessionMock.Object);
            _httpContextMock.SetupGet(x => x.Response).Returns(_responseMock.Object);

            // Set up the controller with mock HttpContext
            _systemUnderTest = new CultureController
            {
                ControllerContext = { HttpContext = _httpContextMock.Object }
            };

        }

        [TestMethod]
        public void CultureController_UpdateCulture_RedirectsToReturnUrlWithCulture()
        {
            // Arrange
            var cultureBytes = Encoding.UTF8.GetBytes(CultureEn);

            // Act
            var result = _systemUnderTest.UpdateCulture(CultureEn, ReturnUrl);

            // Assert
            using (new AssertionScope())
            {
                // Verify session set
                _sessionMock.Verify(x => x.Set(CultureKey, cultureBytes), Times.Once);

                // Verify headers
                var headers = _responseMock.Object.Headers;
                headers["Cache-Control"].ToString().Should().Be("no-store, no-cache, must-revalidate, max-age=0");
                headers["Pragma"].ToString().Should().Be("no-cache");
                headers["Expires"].ToString().Should().Be("-1");

                // Verify redirection
                result.Url.Should().Be(ReturnUrl);
            }

        }
    }
}
