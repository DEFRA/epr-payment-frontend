using AutoFixture;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Infrastructure;
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
            Action act = () => _ = new GovPaySuccessController(_dashboardConfigurationMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'dashboardConfiguration')");
        }

        [TestMethod,AutoMoqData]
        public void Index_WithInvalidModelState_ShouldRedirectToPaymentError(
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
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidViewModel);
            }

        }

        [TestMethod, AutoMoqData]
        public void Index_WhenViewModelIsNull_ShouldRedirectToPaymentError(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] DashboardConfiguration _dashboardConfiguration)
        {
            // Arrange
            CompletePaymentViewModel? _completePaymentViewModel = null;
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfiguration);
            var controller = new GovPaySuccessController(_dashboardConfigurationMock.Object);

            // Act
            var result = controller.Index(_completePaymentViewModel);

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidViewModel);
            }

        }

        [TestMethod, AutoMoqData]
        public void Index_WithValidModelState_ShouldReturnViewWithCompositeViewModel(
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
                var viewResult = result.Should().BeOfType<ViewResult>().Which;
                var model = viewResult.Model.Should().BeOfType<CompositeViewModel>().Which;
                model.completePaymentViewModel.Should().Be(_completePaymentViewModel);
                model.dashboardConfiguration.Should().Be(_dashboardConfiguration);
            }
        }
    }
}
