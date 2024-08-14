using AutoFixture.MSTest;
using AutoMapper.Configuration.Annotations;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
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
        public void Index_WithInvalidModelState_ShouldRedirectToError(
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
                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult!.ActionName.Should().Be("Index");
                redirectResult.ControllerName.Should().Be("Error");
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidViewModel);
            }

        }

        [TestMethod, AutoMoqData]
        public void Index_WithValidModelState_ShouldReturnView(
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
        public async Task InitiatePayment_WithInvalidModelState_ShouldLogErrorAndRedirectToError(
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
            var result = await _controller.InitiatePayment(null, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInvalidPaymentRequestDto)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);

                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult!.ActionName.Should().Be("Index");
                redirectResult.ControllerName.Should().Be("Error");
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorInvalidPaymentRequestDto);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePayment_WithValidRequest_ShouldReturnContent(
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
            var result = await _controller.InitiatePayment(_paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ContentResult>();
                var contentResult = result as ContentResult;
                contentResult!.Content.Should().Be(expectedContent);
                contentResult.ContentType.Should().Be("text/html");
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePayment_WithException_ShouldLogErrorAndRedirectToError(
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
            var result = await _controller.InitiatePayment(_paymentRequestDto, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(ExceptionMessages.ErrorInitiatePayment)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);

                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult!.ActionName.Should().Be("Index");
                redirectResult.ControllerName.Should().Be("Error");
                redirectResult!.RouteValues!["message"].Should().Be(exceptionMessage);
            }
        }
    }
}
