using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Infrastructure;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using EPR.Common.Authorization.Models;

namespace EPR.Payment.Portal.UnitTests.Controllers;

[TestClass]
public class GovPayFailureControllerTests
{

    [TestMethod, AutoMoqData]
    public void Constructor_WithValidArguments_ShouldNotThrow(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);

        var act = () =>
        {
            _ = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
                loggerMock.Object);
        };

        act.Should().NotThrow();
    }

    [TestMethod, AutoMoqData]
    public void Constructor_WithNullPaymentsService_ShouldThrowArgumentNullException(
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);

        var act = () => _ = new GovPayFailureController(null!, dashboardConfigurationMock.Object, loggerMock.Object);

        act.Should().Throw<ArgumentNullException>().WithMessage("*paymentsService*");
    }

    [TestMethod, AutoMoqData]
    public void Constructor_WithNullDashboardConfiguration_ShouldThrowArgumentNullException(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns((DashboardConfiguration)null!);

        var act = () => _ = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);

        act.Should().Throw<ArgumentNullException>().WithMessage("*dashboardConfiguration*");
    }

    [TestMethod, AutoMoqData]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);

        var act = () => _ = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object, null!);

        act.Should().Throw<ArgumentNullException>().WithMessage("*logger*");
    }

    [TestMethod, AutoMoqData]
    public void Index_Get_WithInvalidModelState_ShouldRedirectToPaymentError(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        controller.ModelState.AddModelError("key", "error message");

        var result = controller.Index(null);

        using (new AssertionScope())
        {
            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult.RouteValues.Should().ContainKey("message")
                .WhoseValue.Should().Be(ExceptionMessages.ErrorInvalidViewModel);
        }

    }

    [TestMethod, AutoMoqData]
    public void Index_Get_WithValidModelState_ShouldReturnView(
        string organisationName,
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] CompletePaymentViewModel completePaymentViewModel,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        completePaymentViewModel.Amount = 10000;
        var organisation = new Organisation { Name = organisationName };
        var userDataJson = JsonSerializer.Serialize(new { Organisations = new[] { organisation } });
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
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.ViewData["amount"].Should().Be(completePaymentViewModel.Amount / 100);
            var model = viewResult.Model as CompositeViewModel;
            model!.CompletePaymentViewModel.Should().BeEquivalentTo(completePaymentViewModel);
            model.DashboardConfiguration.Should().BeEquivalentTo(dashboardConfigurationMock.Object.Value);
            model.OrganisationName.Should().Be(organisationName);
        }
    }

    
    [TestMethod, AutoMoqData]
    public void Index_Get_WithValidModelStateAndNoOrganisation_ShouldReturnView(
        string organisationName,
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] CompletePaymentViewModel completePaymentViewModel,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        completePaymentViewModel.Amount = 10000;
        var userDataJson = JsonSerializer.Serialize(new { Organisations = new List<Organisation>() });
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
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.ViewData["amount"].Should().Be(completePaymentViewModel.Amount / 100);
            var model = viewResult.Model as CompositeViewModel;
            model!.CompletePaymentViewModel.Should().BeEquivalentTo(completePaymentViewModel);
            model.DashboardConfiguration.Should().BeEquivalentTo(dashboardConfigurationMock.Object.Value);
            model.OrganisationName.Should().BeNull();
        }
    }
    
    [TestMethod, AutoMoqData]
    public void Index_Get_WithValidModelState_NoUserDataClaim_ShouldSetOrganisationNameNull(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] CompletePaymentViewModel completePaymentViewModel,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        completePaymentViewModel.Amount = 10000;
        var claimsIdentity = new ClaimsIdentity([], "TestAuth");
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(claimsIdentity) };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.Index(completePaymentViewModel);

        using (new AssertionScope())
        {
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model as CompositeViewModel;
            model!.OrganisationName.Should().BeNull();
        }
    }

    [TestMethod, AutoMoqData]
    public void Index_Get_WithValidModelState_EmptyOrganisations_ShouldSetOrganisationNameNull(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] CompletePaymentViewModel completePaymentViewModel,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        completePaymentViewModel.Amount = 10000;
        var userDataJson = JsonSerializer.Serialize(new { Organisations = new List<Organisation>() });
        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.UserData, userDataJson) }, "TestAuth");
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(claimsIdentity) };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = controller.Index(completePaymentViewModel);

        using (new AssertionScope())
        {
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model as CompositeViewModel;
            model!.OrganisationName.Should().BeNull();
        }
    }

    [TestMethod, AutoMoqData]
    public void Index_Get_WithNullViewModel_ShouldRedirectToPaymentError(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);

        var result = controller.Index(null);

        using (new AssertionScope())
        {
            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult.RouteValues.Should().ContainKey("message")
                .WhoseValue.Should().Be(ExceptionMessages.ErrorInvalidViewModel);
        }
    }

    [TestMethod, AutoMoqData]
    public async Task Index_Post_WithInvalidModelState_ShouldLogErrorAndRedirectToPaymentError(
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        controller.ModelState.AddModelError("key", "error message");
        var cancellationToken = CancellationToken.None;

        var result = await controller.Index(null, cancellationToken);

        using (new AssertionScope())
        {
            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInvalidPaymentRequestDto)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult.RouteValues.Should().ContainKey("message");
            redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidPaymentRequestDto);
        }
    }

    [TestMethod, AutoMoqData]
    public async Task Index_Post_WhenPaymentServiceSucceeds_ShouldReturnContentAndShouldNotLogAnError(
        [Frozen] PaymentRequestDto paymentRequestDto,
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
            
        var expectedContent = "<html><body>Payment Initiated</body></html>";
        paymentsServiceMock.Setup(service => service.InitiatePaymentAsync(paymentRequestDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContent);

        var result = await controller.Index(paymentRequestDto, CancellationToken.None);

        using (new AssertionScope())
        {
            var contentResult = result.Should().BeOfType<ContentResult>().Which;
            contentResult.ContentType.Should().Be("text/html");
            contentResult.Content.Should().Be(expectedContent);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Never);
        }
    }

    [TestMethod, AutoMoqData]
    public async Task Index_Post_WhenServiceThrowsException_ShouldRedirectToPaymentError(
        [Frozen] PaymentRequestDto paymentRequestDto,
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        var exceptionMessage = "Payment initiation failed";
        paymentsServiceMock.Setup(service => service.InitiatePaymentAsync(paymentRequestDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var result = await controller.Index(paymentRequestDto, CancellationToken.None);

        using (new AssertionScope())
        {
            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult.RouteValues.Should().ContainKey("message");
            redirectResult!.RouteValues!["message"].Should().Be(exceptionMessage);

            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInitiatePayment)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }
    }

    [TestMethod, AutoMoqData]
    public async Task Index_Post_WithNullRequest_ShouldRedirectToPaymentError(
        [Frozen] PaymentRequestDto paymentRequestDto,
        [Frozen] Mock<IPaymentsService> paymentsServiceMock,
        [Frozen] DashboardConfiguration dashboardConfig,
        [Frozen] Mock<IOptions<DashboardConfiguration>> dashboardConfigurationMock,
        [Frozen] Mock<ILogger<GovPayFailureController>> loggerMock,
        [Greedy] GovPayFailureController controller)
    {
        dashboardConfigurationMock.Setup(x => x.Value).Returns(dashboardConfig);
        controller = new GovPayFailureController(paymentsServiceMock.Object, dashboardConfigurationMock.Object,
            loggerMock.Object);
        controller.ModelState.Clear();
        PaymentRequestDto? request = null;

        var result = await controller.Index(request, CancellationToken.None);

        using (new AssertionScope())
        {
            var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
            redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
            redirectResult.RouteValues.Should().ContainKey("message");
            redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidPaymentRequestDto);

            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInvalidPaymentRequestDto)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }
    }
}