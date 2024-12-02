using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Constants;
using EPR.Payment.Portal.ViewComponents;
using EPR.Payment.Portal.ViewModels;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using CookieOptions = EPR.Payment.Portal.Common.Options.CookieOptions;

namespace EPR.Payment.Portal.UnitTests.ViewComponents
{
    [TestClass]
    public class CookieBannerViewComponentTests
    {
        [TestMethod]
        [AutoMoqData]
        public void Invoke_NoConsentCookieAndNoAcknowledgement_ShouldShowBanner(
        [Frozen] Mock<IOptions<CookieOptions>> mockOptions,
        [Frozen] Mock<IRequestCookieCollection> mockRequestCookies,
        [Frozen] Mock<HttpContext> mockHttpContext,
        [Frozen] Mock<ITempDataDictionary> mockTempData,
        [Frozen] RouteData routeData,
        [Frozen] Mock<ICompositeViewEngine> mockViewEngine,
        [Greedy] CookieBannerViewComponent viewComponent)
        {
            // Arrange
            mockOptions.Setup(x => x.Value).Returns(new CookieOptions
            {
                CookiePolicyCookieName = null,
                SessionCookieName = "",
                CookiePolicyDurationInMonths = 0,
                AntiForgeryCookieName = "",
                TempDataCookie = "",
                TsCookieName = "",
                B2CCookieName = "",
                OpenIdCookieName = "",
                CorrelationCookieName = "",
                AuthenticationCookieName = ""
            });

            mockHttpContext.Setup(ctx => ctx.Request.Path).Returns("/page-with-ack");
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(mockRequestCookies.Object);
            mockTempData.Setup(temp => temp[CookieAcceptance.CookieAcknowledgement]).Returns(CookieAcceptance.Accept);

            // Set up HttpContext.RequestServices to return the mocked ICompositeViewEngine
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ICompositeViewEngine)))
                .Returns(mockViewEngine.Object);
            mockHttpContext.Setup(ctx => ctx.RequestServices).Returns(serviceProviderMock.Object);

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                ["TempData"] = mockTempData.Object
            };

            var viewContext = new ViewContext
            {
                HttpContext = mockHttpContext.Object,
                TempData = mockTempData.Object,
                RouteData = routeData,
                ViewData = viewData,
                View = new Mock<IView>().Object,
            };

            var viewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext,
            };

            viewComponent.ViewComponentContext = viewComponentContext;

            // Act
            var result = viewComponent.Invoke("/return-url") as ViewViewComponentResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var model = result!.ViewData.Model as CookieBannerModel;
                model.Should().NotBeNull();
                model!.ShowAcknowledgement.Should().BeTrue();
                model.AcceptAnalytics.Should().BeTrue();
                model.ShowBanner.Should().BeFalse();
            }
        }

        [TestMethod]
        [AutoMoqData]
        public void Invoke_ControllerIsCookies_ShouldNotShowBannerOrAcknowledgement(
            [Frozen] Mock<IOptions<CookieOptions>> mockOptions,
            [Frozen] Mock<IRequestCookieCollection> mockRequestCookies,
            [Frozen] Mock<HttpContext> mockHttpContext,
            [Frozen] Mock<ITempDataDictionary> mockTempData,
            [Frozen] RouteData routeData,
            [Frozen] Mock<ICompositeViewEngine> mockViewEngine,
            [Greedy] CookieBannerViewComponent viewComponent)
        {
            // Arrange
            mockOptions.Setup(x => x.Value).Returns(new CookieOptions
            {
                CookiePolicyCookieName = "CookiePolicy",
                SessionCookieName = "SessionCookie",
                CookiePolicyDurationInMonths = 3,
                AntiForgeryCookieName = "AntiForgery",
                TempDataCookie = "TempData",
                TsCookieName = "TsCookie",
                B2CCookieName = "B2CCookie",
                OpenIdCookieName = "OpenIdCookie",
                CorrelationCookieName = "CorrelationCookie",
                AuthenticationCookieName = "AuthenticationCookie"
            });

            mockRequestCookies.Setup(c => c["CookiePolicy"]).Returns("True");
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(mockRequestCookies.Object);
            mockHttpContext.Setup(ctx => ctx.Request.Path).Returns("/cookies");
            mockTempData.Setup(temp => temp[CookieAcceptance.CookieAcknowledgement]).Returns(CookieAcceptance.Reject);

            routeData.Values["controller"] = "Cookies";

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ICompositeViewEngine)))
                .Returns(mockViewEngine.Object);
            mockHttpContext.Setup(ctx => ctx.RequestServices).Returns(serviceProviderMock.Object);

            var viewContext = new ViewContext
            {
                HttpContext = mockHttpContext.Object,
                TempData = mockTempData.Object,
                RouteData = routeData,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()),
                View = new Mock<IView>().Object,
            };

            var viewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext,
            };
            viewComponent.ViewComponentContext = viewComponentContext;

            // Act
            var result = viewComponent.Invoke("/return-url") as ViewViewComponentResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var model = result!.ViewData.Model as CookieBannerModel;
                model.Should().NotBeNull();
                model!.ShowBanner.Should().BeFalse();
                model.ShowAcknowledgement.Should().BeFalse();
            }
        }

        [TestMethod]
        [AutoMoqData]
        public void Invoke_ConsentCookieExists_ShouldNotShowBanner(
            [Frozen] Mock<IOptions<CookieOptions>> mockOptions,
            [Frozen] Mock<IRequestCookieCollection> mockRequestCookies,
            [Frozen] Mock<HttpContext> mockHttpContext,
            [Frozen] Mock<ITempDataDictionary> mockTempData,
            [Frozen] RouteData routeData,
            [Frozen] Mock<ICompositeViewEngine> mockViewEngine,
            [Greedy] CookieBannerViewComponent viewComponent)
        {
            // Arrange
            mockOptions.Setup(x => x.Value).Returns(new CookieOptions
            {
                CookiePolicyCookieName = "CookiePolicy",
                SessionCookieName = "SessionCookie",
                CookiePolicyDurationInMonths = 3,
                AntiForgeryCookieName = "AntiForgery",
                TempDataCookie = "TempData",
                TsCookieName = "TsCookie",
                B2CCookieName = "B2CCookie",
                OpenIdCookieName = "OpenIdCookie",
                CorrelationCookieName = "CorrelationCookie",
                AuthenticationCookieName = "AuthenticationCookie"
            });

            mockRequestCookies.Setup(c => c["CookiePolicy"]).Returns("True");
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(mockRequestCookies.Object);
            mockHttpContext.Setup(ctx => ctx.Request.Path).Returns("/another-page");
            mockTempData.Setup(temp => temp[CookieAcceptance.CookieAcknowledgement]).Returns((string)null);

            routeData.Values["controller"] = "Home";

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ICompositeViewEngine)))
                .Returns(mockViewEngine.Object);
            mockHttpContext.Setup(ctx => ctx.RequestServices).Returns(serviceProviderMock.Object);

            var viewContext = new ViewContext
            {
                HttpContext = mockHttpContext.Object,
                TempData = mockTempData.Object,
                RouteData = routeData,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()),
                View = new Mock<IView>().Object,
            };

            var viewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext,
            };
            viewComponent.ViewComponentContext = viewComponentContext;

            // Act
            var result = viewComponent.Invoke("/return-url") as ViewViewComponentResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var model = result!.ViewData.Model as CookieBannerModel;
                model.Should().NotBeNull();
                model!.ShowBanner.Should().BeFalse();
                model.ShowAcknowledgement.Should().BeFalse();
            }
        }
    }
}
