using EPR.Payment.Portal.Controllers.Culture;
using Microsoft.AspNetCore.Http;
using Moq;

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

        [TestInitialize]
        public void Setup()
        {
            _responseCookiesMock = new Mock<IResponseCookies>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();

            // Set up the controller with mock HttpContext
            _systemUnderTest = new CultureController
            {
                ControllerContext = { HttpContext = _httpContextMock.Object }
            };

            _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);
        }
        //[TestMethod]
        //public void CultureController_UpdateCulture_WithValidReturnUrl_RedirectsToReturnUrl()
        //{
        //    // Arrange
        //    const string validReturnUrl = "/valid-path";
        //    const string cultureEn = "en";
        //    var cultureBytes = Encoding.UTF8.GetBytes(cultureEn);

        //    _httpContextMock
        //        .Setup(x => x.Response.Cookies)
        //        .Returns(_responseCookiesMock.Object);

        //    // Act
        //    var result = _systemUnderTest.UpdateCulture(cultureEn, validReturnUrl) as LocalRedirectResult;

        //    // Assert
        //    using (new AssertionScope())
        //    {
        //        result.Should().NotBeNull(); // Ensure result is not null
        //        result!.Url.Should().Be(validReturnUrl); // Ensure it redirects to the valid URL
        //        _sessionMock.Verify(x => x.Set(Language.SessionLanguageKey, cultureBytes), Times.Once);
        //    }
        //}

        //[TestMethod]
        //public void CultureController_UpdateCulture_WithInvalidReturnUrl_DefaultsToRoot()
        //{
        //    // Arrange
        //    const string invalidReturnUrl = "http://malicious-site.com";
        //    const string expectedReturnUrl = "/";
        //    const string cultureEn = "en";
        //    var cultureBytes = Encoding.UTF8.GetBytes(cultureEn);

        //    _httpContextMock
        //        .Setup(x => x.Response.Cookies)
        //        .Returns(_responseCookiesMock.Object);

        //    // Act
        //    var actionResult = _systemUnderTest.UpdateCulture(cultureEn, invalidReturnUrl);

        //    // Assert
        //    using (new AssertionScope())
        //    {
        //        // Cast result to LocalRedirectResult to access Url property
        //        var localRedirectResult = actionResult as LocalRedirectResult;
        //        localRedirectResult.Should().NotBeNull(); // Ensure result is not null
        //        localRedirectResult!.Url.Should().Be(expectedReturnUrl); // Verify redirect URL is correct
        //        _sessionMock.Verify(x => x.Set(Language.SessionLanguageKey, cultureBytes), Times.Once);
        //    }
        //}


    }
}
