using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Constants;
using EPR.Payment.Portal.Controllers.Cookies;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using CookieOptions = EPR.Payment.Portal.Common.Options.CookieOptions;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class CookiesControllerTests
    {
        private Mock<HttpContext>? _httpContextMock;
        private Mock<IRequestCookieCollection>? _requestCookiesMock;
        private Mock<IResponseCookies>? _responseCookiesMock;
        private Mock<ICookieService>? _mockCookieService;
        private Mock<IOptions<CookieOptions>>? _mockCookieOptions;
        private Mock<IOptions<GoogleAnalyticsOptions>>? _mockGoogleAnalyticsOptions;
        private CookiesController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            _httpContextMock = new Mock<HttpContext>();
            _requestCookiesMock = new Mock<IRequestCookieCollection>();
            _responseCookiesMock = new Mock<IResponseCookies>();

            _mockCookieService = new Mock<ICookieService>();
            _mockCookieOptions = new Mock<IOptions<CookieOptions>>();
            _mockGoogleAnalyticsOptions = new Mock<IOptions<GoogleAnalyticsOptions>>();

            _mockCookieOptions.Setup(x => x.Value).Returns(new CookieOptions
            {
                SessionCookieName = "SessionCookie",
                CookiePolicyCookieName = "CookiePolicy",
                AntiForgeryCookieName = "AntiForgery",
                TempDataCookie = "TempData",
                TsCookieName = "TsCookie",
                B2CCookieName = "B2CCookie",
                OpenIdCookieName = "OpenIdCookie",
                CorrelationCookieName = "CorrelationCookie",
                AuthenticationCookieName = "AuthenticationCookie"
            });

            var googleAnalyticsOptions = CreateGoogleAnalyticsOptions();
            _mockGoogleAnalyticsOptions.Setup(x => x.Value).Returns(googleAnalyticsOptions);

            _controller = new CookiesController(
                _mockCookieService.Object,
                _mockCookieOptions.Object,
                _mockGoogleAnalyticsOptions.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };

            _httpContextMock.Setup(x => x.Request.Cookies).Returns(_requestCookiesMock.Object);
            _httpContextMock.Setup(x => x.Response.Cookies).Returns(_responseCookiesMock.Object);
        }

        private static GoogleAnalyticsOptions CreateGoogleAnalyticsOptions()
        {
            return new GoogleAnalyticsOptions
            {
                CookiePrefix = "_ga",
                MeasurementId = "G-12345678",
                TagManagerContainerId = "GTM-1234567"
            };
        }

        [TestMethod]
        public void Detail_GetRequestWithValidReturnUrl_ShouldReturnViewResultWithModel()
        {
            const string returnUrl = "/payment";
            _mockCookieService?.Setup(x => x.HasUserAcceptedCookies(It.IsAny<IRequestCookieCollection>())).Returns(true);

            var result = _controller?.Detail(returnUrl) as ViewResult;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var model = result!.Model as CookieDetailViewModel;
                model.Should().NotBeNull();
                model!.ReturnUrl.Should().Be(returnUrl);
                model.CookiesAccepted.Should().BeTrue();
                model.ShowAcknowledgement.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Detail_InvalidReturnUrl_ShouldReturnBadRequest()
        {
            // Arrange
            const string invalidReturnUrl = "invalid-url"; // Invalid URL format (e.g., doesn't start with '/')

            // Act
            var result = _controller!.Detail(invalidReturnUrl) as BadRequestObjectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                result.Value.Should().Be("Invalid return URL.");
            }
        }

        [TestMethod]
        public void Detail_EmptyReturnUrl_ShouldReturnBadRequest()
        {
            // Arrange
            const string emptyReturnUrl = ""; // Simulate an empty return URL

            // Act
            var result = _controller!.Detail(emptyReturnUrl) as BadRequestObjectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                result.Value.Should().Be("Invalid return URL.");
            }
        }

        [TestMethod]
        public void Detail_GetRequestWithInvalidReturnUrl_ShouldRedirectToDefaultUrl()
        {
            const string invalidReturnUrl = "/invalid-url";
            const string expectedDefaultUrl = "~/";

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns(expectedDefaultUrl);
            _controller!.Url = mockUrlHelper.Object;

            var result = _controller.Detail(invalidReturnUrl) as ViewResult;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var model = result!.Model as CookieDetailViewModel;
                model.Should().NotBeNull();
                model!.ReturnUrl.Should().Be(expectedDefaultUrl);
                mockUrlHelper.Verify(u => u.Content("~/"), Times.Once);
            }
        }

        [TestMethod]
        public void Detail_GetRequestWithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            const string returnUrl = "/report-data";
            _controller!.ModelState.AddModelError("returnUrl", "Invalid return URL");

            // Act
            var result = _controller.Detail(returnUrl) as BadRequestObjectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

                // Validate that the ModelState contains the expected error
                var modelStateErrors = result.Value as SerializableError;
                modelStateErrors.Should().NotBeNull();
                modelStateErrors!.ContainsKey("returnUrl").Should().BeTrue();
                modelStateErrors["returnUrl"].Should().BeOfType<string[]>();
                ((string[])modelStateErrors["returnUrl"]).Should().Contain("Invalid return URL");
            }
        }

        [TestMethod]
        public void Detail_PostRequestWithAcceptedCookies_ShouldSetCookiesAndReturnViewResult()
        {
            const string returnUrl = "/payment";
            const string cookiesAccepted = CookieAcceptance.Accept;

            var requestCookiesMock = new Mock<IRequestCookieCollection>();
            _httpContextMock?.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            _httpContextMock?.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

            var tempData = new TempDataDictionary(_httpContextMock!.Object, Mock.Of<ITempDataProvider>())
            {
                [CookieAcceptance.CookieAcknowledgement] = null
            };
            _controller!.TempData = tempData;

            var result = _controller.Detail(returnUrl, cookiesAccepted) as ViewResult;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                _mockCookieService?.Verify(
                    s => s.SetCookieAcceptance(
                        cookiesAccepted == CookieAcceptance.Accept,
                        requestCookiesMock.Object,
                        responseCookiesMock.Object),
                    Times.Once);
                tempData[CookieAcceptance.CookieAcknowledgement].Should().Be(cookiesAccepted);
            }
        }

        [TestMethod]
        public void UpdateAcceptance_ValidCookies_ShouldSetCookieAcceptanceAndRedirectToReturnUrl()
        {
            const string returnUrl = "/create-account";
            const string cookies = CookieAcceptance.Accept;

            var requestCookiesMock = new Mock<IRequestCookieCollection>();
            _httpContextMock?.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            _httpContextMock?.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

            var tempData = new TempDataDictionary(_httpContextMock!.Object, Mock.Of<ITempDataProvider>())
            {
                [CookieAcceptance.CookieAcknowledgement] = null
            };
            _controller!.TempData = tempData;

            var result = _controller.UpdateAcceptance(returnUrl, cookies) as LocalRedirectResult;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.Url.Should().Be(returnUrl); // Ensure the result is cast and check the Url property
                _mockCookieService?.Verify(
                    s => s.SetCookieAcceptance(
                        cookies == CookieAcceptance.Accept,
                        requestCookiesMock.Object,
                        responseCookiesMock.Object),
                    Times.Once);
                tempData[CookieAcceptance.CookieAcknowledgement].Should().Be(cookies);
            }
        }

        [TestMethod]
        public void AcknowledgeAcceptance_ShouldRedirectToReturnUrl()
        {
            // Arrange
            const string returnUrl = "/manage-account";

            // Act
            var result = _controller?.AcknowledgeAcceptance(returnUrl) as LocalRedirectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.Url.Should().Be(returnUrl);
            }
        }

        [TestMethod]
        public void AcknowledgeAcceptance_ModelStateInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            const string invalidReturnUrl = ""; // Simulate invalid input
            _controller!.ModelState.AddModelError("returnUrl", "Invalid input");

            // Act
            var result = _controller.AcknowledgeAcceptance(invalidReturnUrl) as BadRequestObjectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                result.Value.Should().Be("Invalid input.");
            }
        }

        [TestMethod]
        public void Detail_CookieAccepted_SetsTempData()
        {
            const string returnUrl = "/payment";
            var tempData = new TempDataDictionary(_httpContextMock!.Object, Mock.Of<ITempDataProvider>());
            _controller!.TempData = tempData;

            var result = _controller.Detail(returnUrl, CookieAcceptance.Accept) as ViewResult;

            using (new AssertionScope())
            {
                tempData[CookieAcceptance.CookieAcknowledgement].Should().Be(CookieAcceptance.Accept);
                result.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void Detail_InvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            const string returnUrl = "/payment";
            const string cookiesAccepted = "InvalidValue"; // Simulate invalid data

            // Simulate invalid model state
            _controller!.ModelState.AddModelError("cookiesAccepted", "Invalid value");

            // Act
            var result = _controller.Detail(returnUrl, cookiesAccepted) as BadRequestObjectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                result.Value.Should().Be("Invalid data provided."); // Update with the expected error message
            }
        }

        [TestMethod]
        public void AcknowledgeAcceptance_ReturnUrlIsNull_ShouldRedirectToDefaultUrl()
        {
            // Arrange
            string returnUrl = null!;

            // Act
            var result = _controller!.AcknowledgeAcceptance(returnUrl) as LocalRedirectResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.Url.Should().Be("~/");
            }
        }
    }
}