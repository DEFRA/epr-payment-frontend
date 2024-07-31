using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.MSTest;
using AutoMapper;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Response;
using EPR.Payment.Portal.Common.Enums;
using EPR.Payment.Portal.Common.Profiles;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Services;
using EPR.Payment.Portal.Services.Interfaces;
using Fare;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class PaymentsServiceTests
    {
        private IFixture? _fixture;
        private Mock<IHttpPaymentFacade> _httpPaymentFacadeMock = null!;
        private IPaymentsService _service = null!;
        private IMapper? _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            var throwingRecursionBehaviors = _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList();
            foreach (var behavior in throwingRecursionBehaviors)
            {
                _fixture.Behaviors.Remove(behavior);
            }
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _httpPaymentFacadeMock = _fixture.Freeze<Mock<IHttpPaymentFacade>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PaymentProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _service = new PaymentsService(
                _mapper,
                _httpPaymentFacadeMock.Object);
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePaymentAsync_CallsEndpointSuccesfully(
            [Frozen] Guid externalPaymentId,
            [Frozen] CompletePaymentResponseDto completePaymentResponseDto)
        {
            // Arrange
            _httpPaymentFacadeMock.Setup(s => s.CompletePaymentAsync(externalPaymentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(completePaymentResponseDto);

            // Act
            var result = await _service.CompletePaymentAsync(externalPaymentId, new CancellationToken());

            // Assert
            result.Should().BeEquivalentTo(completePaymentResponseDto);
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePayment_NullExternalPaymentId_ThrowsArgumentException()
        {
            // Act & Assert
            await _service.Invoking(async s => await s.CompletePaymentAsync(Guid.Empty, new CancellationToken()))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage(ExceptionMessages.ErrorEcternalPaymentIdEmpty);
        }

        [TestMethod, AutoMoqData]
        public async Task CompletePayment_PaymentStatusNotFound_ThrowsPaymentStatusNotFoundException(
            [Frozen] Guid externalPaymentId)
        {
            // Arrange
            var completePaymentResponseDto = new CompletePaymentResponseDto
            {
                Reference = null
            };

            _httpPaymentFacadeMock.Setup(s => s.CompletePaymentAsync(externalPaymentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(completePaymentResponseDto);

            // Act & Assert
            await _service.Invoking(async s => await s.CompletePaymentAsync(externalPaymentId, new CancellationToken()))
                .Should().ThrowAsync<Exception>().WithMessage(ExceptionMessages.ErrorRetrievingCompletePayment);
        }
    }
}
