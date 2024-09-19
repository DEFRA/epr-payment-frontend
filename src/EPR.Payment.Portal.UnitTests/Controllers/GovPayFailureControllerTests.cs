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

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class GovPayFailureControllerTests
    {

        [TestMethod, AutoMoqData]
        public void Constructor_WithValidArguments_ShouldNotThrow(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);

            // Act
            Action act = () => new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);

            // Assert
            act.Should().NotThrow();
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WithNullPaymentsService_ShouldThrowArgumentNullException(
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);

            // Act
            Action act = () => new GovPayFailureController(null!, _dashboardConfigurationMock.Object, _loggerMock.Object);


            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*paymentsService*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WithNullDashboardConfiguration_ShouldThrowArgumentNullException(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns((DashboardConfiguration)null!);


            // Act
            Action act = () => new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*dashboardConfiguration*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);

            // Act
            Action act = () => new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*logger*");
        }

        [TestMethod, AutoMoqData]
        public void Index_Get_WithInvalidModelState_ShouldRedirectToPaymentError(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);
            _controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = _controller.Index(null);

            // Assert
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
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] CompletePaymentViewModel _completePaymentViewModel,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);
            _completePaymentViewModel.Amount = 10000;

            // Act
            var result = _controller.Index(_completePaymentViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ViewResult>();
                var viewResult = result as ViewResult;
                viewResult!.ViewData["amount"].Should().Be(_completePaymentViewModel.Amount / 100);
                var model = viewResult.Model as CompositeViewModel;
                model!.completePaymentViewModel.Should().BeEquivalentTo(_completePaymentViewModel);
                model.dashboardConfiguration.Should().BeEquivalentTo(_dashboardConfigurationMock.Object.Value);
            }
        }

        [TestMethod, AutoMoqData]
        public void Index_Get_WithNullViewModel_ShouldRedirectToPaymentError(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);

            // Act
            var result = _controller.Index(null);

            // Assert
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
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);
            _controller.ModelState.AddModelError("key", "error message");
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _controller.Index(null, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                _loggerMock.Verify(logger => logger.Log(
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
            [Frozen] PaymentRequestDto _paymentRequestDto,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);
            
            var expectedContent = "<html><body>Payment Initiated</body></html>";
            _paymentsServiceMock.Setup(service => service.InitiatePaymentAsync(_paymentRequestDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(expectedContent);

            // Act
            var result = await _controller.Index(_paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                var contentResult = result.Should().BeOfType<ContentResult>().Which;
                contentResult.ContentType.Should().Be("text/html");
                contentResult.Content.Should().Be(expectedContent);

                _loggerMock.Verify(
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
            [Frozen] PaymentRequestDto _paymentRequestDto,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);
            var exceptionMessage = "Payment initiation failed";
            _paymentsServiceMock.Setup(service => service.InitiatePaymentAsync(_paymentRequestDto, It.IsAny<CancellationToken>()))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Index(_paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult.RouteValues.Should().ContainKey("message");
                redirectResult!.RouteValues!["message"].Should().Be(exceptionMessage);

                _loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInitiatePayment)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_Post_WithNullRequest_ShouldRedirectToPaymentError(
            [Frozen] PaymentRequestDto _paymentRequestDto,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] DashboardConfiguration _dashboardConfig,
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<ILogger<GovPayFailureController>> _loggerMock,
            [Greedy] GovPayFailureController _controller)
        {
            // Arrange
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(_dashboardConfig);
            _controller = new GovPayFailureController(_paymentsServiceMock.Object, _dashboardConfigurationMock.Object,
                _loggerMock.Object);
            _controller.ModelState.Clear();
            PaymentRequestDto? request = null;

            // Act
            var result = await _controller.Index(request, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult.RouteValues.Should().ContainKey("message");
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidPaymentRequestDto);

                _loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInvalidPaymentRequestDto)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
            }

        }
    }
}
