using AutoFixture;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
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

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenDashboardConfigurationIsNull(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns((DashboardConfiguration)null!);

            // Act
            Action act = () => new GovPaySuccessController(_dashboardConfigurationMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'dashboardConfiguration')");
        }

        [TestMethod,AutoMoqData]
        public void Index_WithInvalidModelState_ShouldRedirectToError(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] DashboardConfiguration _dashboardConfiguration)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfiguration);
            var controller = new GovPaySuccessController(_dashboardConfigurationMock.Object);
            controller.ModelState.AddModelError("Test", "Test error");

            // Act
            var result = controller.Index(null);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult.Should().NotBeNull();
                redirectResult!.ActionName.Should().Be("Index");
                redirectResult.ControllerName.Should().Be("Error");
                redirectResult.RouteValues.Should().ContainKey("message");
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidViewModel);
            }

        }

        [TestMethod, AutoMoqData]
        public void Index_WithValidModelState_ShouldReturnViewResult(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] DashboardConfiguration _dashboardConfiguration,
            [Frozen] CompletePaymentViewModel _completePaymentViewModel)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfiguration);
            var controller = new GovPaySuccessController(_dashboardConfigurationMock.Object);

            // Act
            var result = controller.Index(_completePaymentViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ViewResult>();
                var viewResult = result as ViewResult;
                viewResult.Should().NotBeNull();
                viewResult!.Model.Should().BeOfType<CompositeViewModel>();
                var compositeViewModel = viewResult.Model as CompositeViewModel;
                compositeViewModel!.completePaymentViewModel.Should().Be(_completePaymentViewModel);
                compositeViewModel.dashboardConfiguration.Should().Be(_dashboardConfiguration);
            }
        }
    }
}
