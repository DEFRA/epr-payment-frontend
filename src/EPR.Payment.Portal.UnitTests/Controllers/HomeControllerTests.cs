using AutoFixture;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<IOptions<Service>>? _configMock;
        private Mock<DashboardConfiguration> mockDashboardConfig = null!;
        private Mock<IOptions<DashboardConfiguration>> mockOptions = null!;

        [TestInitialize]
        public void SetUp()
        {
            mockDashboardConfig = new Mock<DashboardConfiguration>();
            mockDashboardConfig.SetupAllProperties();
            mockDashboardConfig.Object.MenuUrl.Url = "https://menuurl.com";
            mockDashboardConfig.Object.MenuUrl.Description = "Menu Url";
            mockDashboardConfig.Object.BackUrl.Url = "https://backurl.com";
            mockDashboardConfig.Object.BackUrl.Description = "Back Url";
            mockDashboardConfig.Object.FeedbackUrl.Url = "https://feedbackurl.com";
            mockDashboardConfig.Object.FeedbackUrl.Description = "Feedback Url";
            mockDashboardConfig.Object.OfflinePaymentUrl.Url = "https://offlinepayment.com";
            mockDashboardConfig.Object.OfflinePaymentUrl.Description = "OfflinePayment Url";

            mockOptions = new Mock<IOptions<DashboardConfiguration>>();
            mockOptions.Setup(o => o.Value).Returns(mockDashboardConfig.Object);
            
        }

        [TestMethod, AutoMoqData]
        public void Constructor_Should_Throw_ArgumentNullException_When_Config_Is_Null()
        {
            // Act
            mockOptions.Setup(o => o.Value).Returns((DashboardConfiguration)null!);
            Action act = () => new HomeController(mockOptions.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*dashboardConfiguration*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_Should_Initialize_When_Config_Is_Not_Null(IOptions<DashboardConfiguration> config)
        {
            // Act
            var controller = new HomeController(mockOptions.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [TestMethod, AutoMoqData]
        public void Index_Should_Return_View_With_Correct_Configuration()
        {
            // Arrange
            var fixture = new Fixture();
            var dashboardConfig = fixture.Build<DashboardConfiguration>()
                .With(x => x.BackUrl, new Service() { Url = "https://backurl.com" })
                .With(x => x.OfflinePaymentUrl, new Service() { Url = "https://offlinepayment.com" })
                .Create();
            var options = Options.Create(dashboardConfig);

            var controller = new HomeController(mockOptions.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            //result!.Model.Should().Be(dashboardConfig);
            result.Should().BeOfType<ViewResult>();
            result!.Model.Should().Be(mockDashboardConfig.Object);
            //controller.ViewBag.BackUrl.Should().Be("https://backurl.com");
            //controller.ViewBag.OfflinePaymentUrl.Should().Be("https://offlinepayment.com");
        }
    }
}
