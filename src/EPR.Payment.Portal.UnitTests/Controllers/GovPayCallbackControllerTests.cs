using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Exceptions;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Infrastructure;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class GovPayCallbackControllerTests
    {

        [TestMethod, AutoMoqData]
        public async Task Index_WithInvalidModelState_ShouldLogErrorAndReturnRedirectToErrorRoute(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Arrange
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            _controller.ModelState.AddModelError("id", "Invalid Guid");
            var id = Guid.Empty;

            // Act
            var result = await _controller.Index(id, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                _testLogger.LogEntries.Should().ContainSingle()
                            .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorExternalPaymentIdEmpty));

                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorExternalPaymentIdEmpty);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithValidId_ShoulReturnRedirectToSuccessRoute(
            [Frozen] Guid id,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Arrange
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            CompletePaymentViewModel completePaymentViewModel = new CompletePaymentViewModel()
            {
                Status = Common.Enums.PaymentStatus.Success,
                Reference = "Reference"
            };

            _paymentsServiceMock.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(completePaymentViewModel);


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentSuccess);
                redirectResult!.RouteValues!["Status"].Should().BeEquivalentTo(completePaymentViewModel.Status);
                redirectResult!.RouteValues!["Reference"].Should().BeEquivalentTo(completePaymentViewModel.Reference);
                redirectResult!.RouteValues!["Message"].Should().BeEquivalentTo(completePaymentViewModel.Message);
                redirectResult!.RouteValues!["UserId"].Should().BeEquivalentTo(completePaymentViewModel.UserId);
                redirectResult!.RouteValues!["OrganisationId"].Should().BeEquivalentTo(completePaymentViewModel.OrganisationId);
                redirectResult!.RouteValues!["Regulator"].Should().BeEquivalentTo(completePaymentViewModel.Regulator);
                redirectResult!.RouteValues!["Amount"].Should().BeEquivalentTo(completePaymentViewModel.Amount);
                redirectResult!.RouteValues!["Email"].Should().BeEquivalentTo(completePaymentViewModel.Email);
                _paymentsServiceMock.Verify(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithValidIdButFailedPayment_ShoulReturnRedirectToFailureRoute(
            [Frozen] Guid id,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Arrange
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            CompletePaymentViewModel completePaymentViewModel = new CompletePaymentViewModel()
            {
                Status = Common.Enums.PaymentStatus.Failed,
                Reference = "Reference"
            };

            _paymentsServiceMock.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(completePaymentViewModel);


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.Paymentfailure);
                redirectResult!.RouteValues!["Status"].Should().BeEquivalentTo(completePaymentViewModel.Status);
                redirectResult!.RouteValues!["Reference"].Should().BeEquivalentTo(completePaymentViewModel.Reference);
                redirectResult!.RouteValues!["Message"].Should().BeEquivalentTo(completePaymentViewModel.Message);
                redirectResult!.RouteValues!["UserId"].Should().BeEquivalentTo(completePaymentViewModel.UserId);
                redirectResult!.RouteValues!["OrganisationId"].Should().BeEquivalentTo(completePaymentViewModel.OrganisationId);
                redirectResult!.RouteValues!["Regulator"].Should().BeEquivalentTo(completePaymentViewModel.Regulator);
                redirectResult!.RouteValues!["Amount"].Should().BeEquivalentTo(completePaymentViewModel.Amount);
                redirectResult!.RouteValues!["Email"].Should().BeEquivalentTo(completePaymentViewModel.Email);
                _paymentsServiceMock.Verify(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_ServiceThrowsException_ShoulLogExceptionAndReturnRedirectToErrorRoute(
            [Frozen] Guid id,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Arrange
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            _paymentsServiceMock.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test Exception"));
            string loggedException = string.Concat("Error completing payment for ID ", id.ToString());


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult!.RouteValues!["message"].Should().Be("Test Exception");

                _testLogger.LogEntries.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo((LogLevel.Error, loggedException));
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithEmptyGuid_ShoulLogExceptionAndReturnRedirectToErrorRoute(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Act
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            var result = await _controller.Index(Guid.Empty, It.IsAny<CancellationToken>()) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorExternalPaymentIdEmpty);

                _testLogger.LogEntries.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo((LogLevel.Error, ExceptionMessages.ErrorExternalPaymentIdEmpty));
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WhenServiceThrowsException_ShoulLogExceptionAndReturnRedirectToErrorRoute(
            [Frozen] Guid id,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Arrange
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            var exception = new ServiceException("Service exception");
            _paymentsServiceMock.Setup(ps => ps.CompletePaymentAsync(id, It.IsAny<CancellationToken>()))
                                .ThrowsAsync(exception);
            string loggedException = string.Concat("Error completing payment for ID ", id.ToString());

            // Act
            var result = await _controller.Index(id, CancellationToken.None) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                var redirectResult = result.Should().BeOfType<RedirectToRouteResult>().Which;
                redirectResult.RouteName.Should().Be(RouteNames.GovPay.PaymentError);
                redirectResult!.RouteValues!["message"].Should().Be("Service exception");

                _testLogger.LogEntries.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo((LogLevel.Error, loggedException));
            }
        }
    }
}
