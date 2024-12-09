using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Constants;
using EPR.Payment.Portal.Middleware;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Middleware
{
    [TestClass]
    public class AnalyticsCookieMiddlewareTests
    {
        private Mock<RequestDelegate>? _mockRequestDelegate;
        private Mock<ICookieService>? _mockCookieService;
        private Mock<IOptions<GoogleAnalyticsOptions>>? _mockAnalyticsOptions;

        [TestInitialize]
        public void SetUp()
        {
            _mockRequestDelegate = new Mock<RequestDelegate>();
            _mockCookieService = new Mock<ICookieService>();
            _mockAnalyticsOptions = new Mock<IOptions<GoogleAnalyticsOptions>>();
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldSetHttpContextItems()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var middleware = new AnalyticsCookieMiddleware(_mockRequestDelegate!.Object);
            var hasUserAcceptedCookies = true;
            var tagManagerContainerId = "GTM-XXXX";

            _mockCookieService?.Setup(s => s.HasUserAcceptedCookies(httpContext.Request.Cookies)).Returns(hasUserAcceptedCookies);
            _mockAnalyticsOptions?.Setup(s => s.Value)
                .Returns(new GoogleAnalyticsOptions
                {
                    CookiePrefix = "ga_cookie",
                    MeasurementId = "G-XXXXX",
                    TagManagerContainerId = tagManagerContainerId
                });

            // Act
            await middleware.InvokeAsync(httpContext, _mockCookieService!.Object, _mockAnalyticsOptions!.Object);

            // Assert
            Assert.AreEqual(hasUserAcceptedCookies, httpContext.Items[ContextKeys.UseGoogleAnalyticsCookieKey]);
            Assert.AreEqual(tagManagerContainerId, httpContext.Items[ContextKeys.TagManagerContainerIdKey]);

            // Verify that _next middleware is invoked
            _mockRequestDelegate.Verify(next => next(httpContext), Times.Once);
        }
    }
}