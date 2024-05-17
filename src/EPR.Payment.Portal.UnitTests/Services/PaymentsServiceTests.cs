using AutoMapper;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Dtos.Response.Common;
using EPR.Payment.Portal.Common.Models.Request;
using EPR.Payment.Portal.Common.Models.Response;
using EPR.Payment.Portal.Common.Profiles;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services;
using Moq;
using FluentAssertions;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class PaymentsServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IHttpPaymentsService> _httpPaymentsServiceMock;
        private readonly PaymentsService _paymentsService;

        public PaymentsServiceTests()
        {
            _httpPaymentsServiceMock = new Mock<IHttpPaymentsService>();
            var configuration = SetupAutomapper();
            _mapper = new Mapper(configuration);
            _paymentsService = new PaymentsService(
                _mapper,
                _httpPaymentsServiceMock.Object);
        }
        private MapperConfiguration SetupAutomapper()
        {
            var myProfile = new PaymentProfile();
            return new MapperConfiguration(c => c.AddProfile(myProfile));
        }

        [TestMethod]
        public async Task GetPaymentStatus_ReturnsCorrectViewModel()
        {
            // Arrange
            var paymentId = "123456";
            var expectedResponse = new PaymentStatusResponseDto
            {
                PaymentId = paymentId,
                State = new State { Finished = true },
                Amount = 100,
                Description = "Test Payment"
            };

            _httpPaymentsServiceMock.Setup(s => s.GetPaymentStatus(paymentId)).ReturnsAsync(expectedResponse);

            // Act
            var response = await _paymentsService.GetPaymentStatus(paymentId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(PaymentStatusResponseViewModel));

            _httpPaymentsServiceMock.Verify(s =>s.GetPaymentStatus(paymentId),Times.Once);
        }

        [TestMethod]
        public async Task PaymentStatus_InsertInvalidRequest_Success()
        {
            // Arrange
            var statusViewModel = new PaymentStatusInsertRequestViewModel { Status = "Inserted" };
            var paymentId = "123";

            // Act
            Func<Task> action = async () => await _paymentsService.InsertPaymentStatus(paymentId, statusViewModel);


            // Assert
            await action.Should().NotThrowAsync();
        }
    }
}
