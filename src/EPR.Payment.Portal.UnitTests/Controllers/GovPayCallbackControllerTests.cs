using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Services;
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
        private IFixture? _fixture;
        private Mock<IPaymentsService> _paymentsServiceMock = null!;
        private GovPayCallbackController _controller = null!;
        private Mock<ILogger<GovPayCallbackController>>? _loggerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            _paymentsServiceMock = new Mock<IPaymentsService>();
            _loggerMock = _fixture.Freeze<Mock<ILogger<GovPayCallbackController>>>();
            _controller = new GovPayCallbackController(_paymentsServiceMock.Object, _loggerMock.Object); 
        }

        [TestMethod, AutoMoqData]
        public async Task Index_WithValidId_ShoulReturnCorrectView([Frozen] Guid id)
        {
            // Arrange
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
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("PaymentSuccess");
                _paymentsServiceMock.Verify(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task Index_ServiceThrowsException_ShoulReturnErrorView([Frozen] Guid id)
        {
            // Arrange
            _paymentsServiceMock.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test Exception"));


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("PaymentError");
        }

        [TestMethod]
        public async Task Index_WithEmptyGuidId_ShoulReturnErrorView()
        {
            // Act
            var result = await _controller.Index(Guid.Empty, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("PaymentError");
            }
        }
    }
}
