using AutoFixture;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class GovPaySuccessControllerTests
    {
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

        [TestMethod]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            mockOptions.Setup(o => o.Value).Returns((DashboardConfiguration)null!);
            Action act = () => new GovPaySuccessController(mockOptions.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*dashboardConfiguration*");
        }

        [TestMethod]
        public void Constructor_WhenConfigIsNotNull_ShouldInitialize()
        {
            // Act
            var controller = new GovPaySuccessController(mockOptions.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [TestMethod, AutoMoqData]
        public void Index_WithCorrectConfiguration_ShouldReturnView([Frozen] CompletePaymentViewModel completePaymentResponseViewModel)
        {
            // Arrange
            var fixture = new Fixture();
            var dashboardConfig = fixture.Build<DashboardConfiguration>()
                .With(x => x.BackUrl, new Service() { Url = "https://backurl.com" })
                .Create();
            var options = Options.Create(dashboardConfig);

            var controller = new GovPaySuccessController(mockOptions.Object);

            // Act
            var result = controller.Index(completePaymentResponseViewModel) as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeOfType<ViewResult>();
            }

        }

        [TestMethod]
        public void Index_WithNullViewModel_ShoulReturnErrorView()
        {
            // Arrange
            var controller = new GovPaySuccessController(mockOptions.Object);

            // Act
            var result = controller.Index((CompletePaymentViewModel?)null) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Error");
            }
        }
    }
}
