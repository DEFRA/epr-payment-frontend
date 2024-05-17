using EPR.Payment.Portal.Common.Dtos.Response.Common;
using EPR.Payment.Portal.Common.Models.Request;
using EPR.Payment.Portal.Common.Models.Response;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class PaymentsControllerTests
    {
        private readonly PaymentsController _controller;
        private readonly Mock<IPaymentsService> _paymentsServiceMock;

        public PaymentsControllerTests()
        {
            _paymentsServiceMock = new Mock<IPaymentsService>();
            _controller = new PaymentsController(_paymentsServiceMock.Object);
        }

        [TestMethod]
        public async Task PaymentStatus_ReturnsViewWithViewModel()
        {
            // Arrange
            var paymentId = "12345";
            var expectedResponse = SetupPaymentStatusResponseDto();

            _paymentsServiceMock.Setup(service => service.GetPaymentStatus(paymentId)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.PaymentStatus(paymentId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult? viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewData.Model);

            // check model is expected type
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PaymentStatusResponseViewModel));

            // check view name
            Assert.IsNull(viewResult.ViewName);

            _paymentsServiceMock.Verify(service => service.GetPaymentStatus(paymentId), Times.Once());
        }

        [TestMethod]
        public async Task PaymentStatus_ReturnsNotFound_WhenPaymentIdIsNull()
        {
            var result = await _controller.PaymentStatus(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            _paymentsServiceMock.Verify(service => service.GetPaymentStatus(null), Times.Never());
        }

        [TestMethod]
        public async Task PaymentStatus_SavesWithValidData()
        {
            // Arrange
            var paymentId = "12345";
            var viewModel = new PaymentStatusInsertRequestViewModel { Status = "Inserted" };

            // Act
            var result = await _controller.PaymentStatus(paymentId, viewModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);

            _paymentsServiceMock.Verify(service => service.InsertPaymentStatus(paymentId, viewModel), Times.Once);
        }

        [TestMethod]
        public async Task PaymentStatus_InsertInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var paymentId = "12345";
            //TODO : PS - need to setup tests for exact model for invalid state
            var viewModel = new PaymentStatusInsertRequestViewModel { /* Invalid request data */ };
            _controller.ModelState.AddModelError("PropertyName", "Error message"); // Add a model state error

            // Act
            var result = await _controller.PaymentStatus(paymentId, viewModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("PaymentStatus", redirectToActionResult.ActionName);

            _paymentsServiceMock.Verify(s => s.InsertPaymentStatus(paymentId, viewModel), Times.Never);
        }

        private PaymentStatusResponseViewModel SetupPaymentStatusResponseDto()
        {
            return new PaymentStatusResponseViewModel
            {
                Amount = 14500,
                Reference = "12345",
                Description = "Pay your council tax",
                PaymentId = "no7kr7it1vjbsvb7r402qqrv86",
                Email = "sherlock.holmes@example.com",
                State = new State { Status = "Success", Finished = true },
                Metadata = new Metadata { LedgerCode = "1234", InternalReferenceNumber = 5678 },
                RefundSummary = new RefundSummary { Status = "Refunded", AmountAvailable = 1000, AmountSubmitted = 500 },
                SettlementSummary = new SettlementSummary(), // Ensure correct namespace here
                CardDetails = new CardDetails
                {
                    LastDigitsCardNumber = "1234",
                    FirstDigitsCardNumber = "456",
                    CardholderName = "John Doe",
                    ExpiryDate = "12/23",
                    BillingAddress = new BillingAddress { Line1 = "123 Street", City = "City", Postcode = "12345", Country = "Country" },
                    CardBrand = "Visa",
                    CardType = "Debit",
                    WalletType = "Apple Pay"
                },
                DelayedCapture = true,
                Moto = false,
                ReturnUrl = "https://your.service.gov.uk/completed",
                AuthorisationMode = "3D Secure",
                Links = new Links
                {
                    Self = new Self { Href = "https://example.com/self", Method = "GET" },
                    NextUrl = new NextUrl { Href = "https://example.com/next", Method = "POST" },
                    NextUrlPost = new NextUrlPost { Href = "https://example.com/nextpost", Method = "POST" },
                    Events = new Events { Href = "https://example.com/events", Method = "GET" },
                    Refunds = new Refunds { Href = "https://example.com/refunds", Method = "POST" },
                    Cancel = new Cancel { Href = "https://example.com/cancel", Method = "DELETE" }
                }
            };
        }

    }
}
