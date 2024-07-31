using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class GovPayCallbackControllerTests
    {
        private Mock<IPaymentsService> _paymentsService = null!;
        private GovPayCallbackController _controller = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _paymentsService = new Mock<IPaymentsService>();
            _controller = new GovPayCallbackController(_paymentsService.Object);
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

            _paymentsService.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(completePaymentViewModel);


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("PaymentSuccess");


            _paymentsService.Verify(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod, AutoMoqData]
        public async Task Index_ServiceThrowsException_ShoulReturnErrorView([Frozen] Guid id)
        {
            // Arrange
            _paymentsService.Setup(service => service.CompletePaymentAsync(id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test Exception"));


            // Act
            var result = await _controller.Index(id, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("PaymentError");
        }

    }
}
