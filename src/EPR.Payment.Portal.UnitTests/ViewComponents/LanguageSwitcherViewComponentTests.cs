using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.ViewComponents;
using EPR.Payment.Portal.ViewModels;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using System.Globalization;

namespace EPR.Payment.Portal.UnitTests.ViewComponents
{
    [TestClass]
    public class LanguageSwitcherViewComponentTests
    {

        private Mock<HttpContext> _httpContextMock = null!;
        private Mock<HttpRequest> _httpRequestMock = null!;
        private IOptions<RequestLocalizationOptions> _localizationOptions = null!;
        private Mock<IFeatureManager> _featureManagerMock = null!;
        private LanguageSwitcherViewComponent _viewComponent = null!;

        [TestInitialize]
        public void TestInitialize()
        {

            // Set up Localization options
            var options = new RequestLocalizationOptions();
            options.AddSupportedCultures(Language.English, Language.Welsh);
            _localizationOptions = Microsoft.Extensions.Options.Options.Create(options);

            // Set up Feature Manager mock to enable ShowLanguageSwitcher
            _featureManagerMock = new Mock<IFeatureManager>();
            _featureManagerMock.Setup(x => x.IsEnabledAsync(nameof(FeatureFlags.ShowLanguageSwitcher)))
                .ReturnsAsync(true);

            // Initialize the view component
            _viewComponent = new LanguageSwitcherViewComponent(_localizationOptions, _featureManagerMock.Object);

            // Set up Mock for HttpContext and HttpRequest
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _httpRequestMock.Setup(x => x.Path).Returns("/test");
            _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?test=true"));

            _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
            _httpContextMock.Setup(x => x.Features.Get<IRequestCultureFeature>())
                .Returns(new RequestCultureFeature(new RequestCulture(Language.English), null));

            _viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext { HttpContext = _httpContextMock.Object }
            };
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldRenderCorrectView_WithExpectedModel()
        {
            // Act
            var result = await _viewComponent.InvokeAsync() as ViewViewComponentResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ViewData!.Model.Should().BeEquivalentTo(new LanguageSwitcherModel
                {
                    SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo(Language.English),
                    new CultureInfo(Language.Welsh)
                },
                    CurrentCulture = new CultureInfo(Language.English),
                    ReturnUrl = "~" + "/test" + "?test=true",
                    ShowLanguageSwitcher = true
                });
            }

        }
    }
}
