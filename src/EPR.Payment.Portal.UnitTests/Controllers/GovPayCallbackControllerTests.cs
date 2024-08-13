using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Exceptions;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class GovPayCallbackControllerTests
    {

        [TestMethod, AutoMoqData]
        public async Task Index_WithInvalidModelState_ShouldLogErrorAndRedirectToError(
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

                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult!.ControllerName.Should().Be("Error");
                redirectResult.ActionName.Should().Be("Index");
                redirectResult!.RouteValues!["message"].Should().Be(ExceptionMessages.ErrorExternalPaymentIdEmpty);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithValidId_ShoulReturnCorrectViewAsSuccess(
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
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("GovPaySuccess");
                _paymentsServiceMock.Verify(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithValidId_ShoulReturnCorrectViewAsFailure(
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
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("GovPayFailure");
                _paymentsServiceMock.Verify(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_ServiceThrowsException_ShoulReturnErrorView(
            [Frozen] Guid id,
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Arrange
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            _paymentsServiceMock.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test Exception"));


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Error");
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithEmptyGuidId_ShoulReturnErrorView(
            [Frozen] Mock<IPaymentsService> _paymentsServiceMock,
            [Frozen] TestLogger<GovPayCallbackController> _testLogger,
            [Greedy] GovPayCallbackController _controller)
        {
            // Act
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _testLogger);
            var result = await _controller.Index(Guid.Empty, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Error");
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WhenServiceThrowsException_ShouldLogErrorAndRedirectToError(
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

            // Act
            var result = await _controller.Index(id, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                _testLogger.LogEntries.Should().ContainSingle()
                    .Which.Item2.Should().Contain("Error completing payment for ID");

                result.Should().BeOfType<RedirectToActionResult>();
                var redirectResult = result as RedirectToActionResult;
                redirectResult!.ControllerName.Should().Be("Error");
                redirectResult.ActionName.Should().Be("Index");
                redirectResult!.RouteValues!["message"].Should().Be(exception.Message);
            }
        }
    }
}
