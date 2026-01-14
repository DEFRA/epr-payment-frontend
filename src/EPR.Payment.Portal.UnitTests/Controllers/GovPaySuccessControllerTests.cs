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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using EPR.Common.Authorization.Models;

namespace EPR.Payment.Portal.UnitTests.Controllers;

[TestClass]
public class GovPaySuccessControllerTests
{
    [TestMethod, AutoMoqData]
    public void Constructor_ShouldThrowArgumentNullException_WhenDashboardConfigurationIsNull(
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns((DashboardConfiguration)null!);

        Action act = () => _ = new GovPaySuccessController(dashboardConfigurationMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'dashboardConfiguration')");
    }

    [TestMethod,AutoMoqData]
    public void Index_WithInvalidModelState_ShouldRedirectToPaymentError(
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] DashboardConfiguration dashboardConfiguration)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfiguration);
        var controller = new GovPaySuccessController(dashboardConfigurationMock.Object);
        controller.ModelState.AddModelError("Test", "Test error");

        var result = controller.Index(null);

        using (new AssertionScope())
        {
            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidViewModel);
        }
    }

    [TestMethod, AutoMoqData]
    public void Index_WhenViewModelIsNull_ShouldRedirectToPaymentError(
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] DashboardConfiguration dashboardConfiguration)
    {
        CompletePaymentViewModel? completePaymentViewModel = null;
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfiguration);
        var controller = new GovPaySuccessController(dashboardConfigurationMock.Object);

        var result = controller.Index(completePaymentViewModel);

        using (new AssertionScope())
        {
            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidViewModel);
        }
    }

    [TestMethod, AutoMoqData]
    public void Index_WithValidModelState_ShouldReturnViewWithCompositeViewModel(
        string organisationName,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] DashboardConfiguration dashboardConfiguration,
        [Frozen] CompletePaymentViewModel completePaymentViewModel)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfiguration);
        var controller = new GovPaySuccessController(dashboardConfigurationMock.Object);
        var userDataJson = JsonSerializer.Serialize(new 
        { 
            Organisations = new[]
            {
                new Organisation { Name = organisationName }
            } 
        });
        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.UserData, userDataJson)
        ], "TestAuth");
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = controller.Index(completePaymentViewModel);

        using (new AssertionScope())
        {
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<CompositeViewModel>().Which;
            model.CompletePaymentViewModel.Should().Be(completePaymentViewModel);
            model.DashboardConfiguration.Should().Be(dashboardConfiguration);
            model.OrganisationName.Should().Be(organisationName);
        }
    }

    [TestMethod, AutoMoqData]
    public void Index_WithValidModelState_NoOrganisationDataClaim_ShouldSetOrganisationNameNull(
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] DashboardConfiguration dashboardConfiguration,
        [Frozen] CompletePaymentViewModel completePaymentViewModel)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfiguration);
        var controller = new GovPaySuccessController(dashboardConfigurationMock.Object);
        var userDataJson = JsonSerializer.Serialize(new { Organisations = new List<Organisation>() });
        var claimsIdentity = new ClaimsIdentity([new Claim(ClaimTypes.UserData, userDataJson)], "TestAuth");
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(claimsIdentity) };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.Index(completePaymentViewModel);

        using (new AssertionScope())
        {
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<CompositeViewModel>().Which;
            model.OrganisationName.Should().BeNull();
        }
    }

    [TestMethod, AutoMoqData]
    public void Index_WithValidModelState_EmptyOrganisations_ShouldSetOrganisationNameNull(
        [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
        [Frozen] DashboardConfiguration _dashboardConfiguration,
        [Frozen] CompletePaymentViewModel _completePaymentViewModel)
    {
        _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfiguration);
        var controller = new GovPaySuccessController(_dashboardConfigurationMock.Object);
        var userDataJson = JsonSerializer.Serialize(new 
        { 
            Organisations = new List<Organisation>()
        });
        var claimsIdentity = new ClaimsIdentity([new Claim(ClaimTypes.UserData, userDataJson)], "TestAuth");
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(claimsIdentity) };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.Index(_completePaymentViewModel);

        using (new AssertionScope())
        {
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<CompositeViewModel>().Which;
            model.OrganisationName.Should().BeNull();
        }
    }
}