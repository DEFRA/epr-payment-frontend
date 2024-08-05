using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
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
        private IFixture? _fixture;
        private Mock<DashboardConfiguration> mockDashboardConfig = null!;
        private Mock<IOptions<DashboardConfiguration>> mockOptions = null!;
        private Mock<IPaymentsService> _paymentsServiceMock = null!;
        private Mock<ILogger<GovPayFailureController>> _loggerMock = null!;

        [TestInitialize]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            _paymentsServiceMock = new Mock<IPaymentsService>();

            mockDashboardConfig = new Mock<DashboardConfiguration>();
            mockDashboardConfig.SetupAllProperties();
            mockDashboardConfig.Object.MenuUrl.Url = "https://menuurl.com";
            mockDashboardConfig.Object.MenuUrl.Description = "Menu Url";
            mockDashboardConfig.Object.BackUrl.Url = "https://backurl.com";
            mockDashboardConfig.Object.BackUrl.Description = "Back Url";
            mockDashboardConfig.Object.FeedbackUrl.Url = "https://feedbackurl.com";
            mockDashboardConfig.Object.FeedbackUrl.Description = "Feedback Url";
            mockDashboardConfig.Object.OfflinePaymentUrl.Url = "https://offlinepayment.com";
            mockDashboardConfig.Object.OfflinePaymentUrl.Description = "OfflinePayment Url";

            mockOptions = new Mock<IOptions<DashboardConfiguration>>();
            mockOptions.Setup(o => o.Value).Returns(mockDashboardConfig.Object);
            _loggerMock = _fixture.Freeze<Mock<ILogger<GovPayFailureController>>>();

        }

        [TestMethod]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            mockOptions.Setup(o => o.Value).Returns((DashboardConfiguration)null!);
            Action act = () => new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*dashboardConfiguration*");
        }

        [TestMethod]
        public void Constructor_WhenConfigIsNotNull_ShouldInitialize()
        {
            // Act
            var controller = new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [TestMethod, AutoMoqData]
        public void Index_WithCorrectConfiguration_ShouldReturnView([Frozen] CompletePaymentViewModel completePaymentResponseViewModel)
        {
            // Arrange
            var fixture = new Fixture();
            var dashboardConfig = fixture.Build<DashboardConfiguration>()
                .With(x => x.BackUrl, new Service() { Url = "https://backurl.com" })
                .Create();
            var options = Options.Create(dashboardConfig);

            var controller = new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);

            // Act
            var result = controller.Index(completePaymentResponseViewModel) as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeOfType<ViewResult>();
            }

        }

        [TestMethod]
        public void Index_WithNullViewModel_ShoulReturnErrorView()
        {
            // Arrange
            var controller = new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);

            // Act
            var result = controller.Index((CompletePaymentViewModel?)null) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Error");
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePayment_WithValidRequest_ShoulReturnCorrectView([Frozen] PaymentRequestDto request, [Frozen] string expectedResponseContent)
        {
            // Arrange
            var controller = new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);

            CompletePaymentViewModel completePaymentViewModel = new CompletePaymentViewModel()
            {
                Status = Common.Enums.PaymentStatus.Success,
                Reference = "Reference"
            };

            _paymentsServiceMock.Setup(service => service.InitiatePaymentAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponseContent);


            // Act
            var result = await controller.InitiatePayment(request, It.IsAny<CancellationToken>()) as ContentResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Content.Should().Be(expectedResponseContent);
                result.ContentType.Should().Be("text/html");
                _paymentsServiceMock.Verify(service => service.InitiatePaymentAsync(request, It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [TestMethod, AutoMoqData]
        public async Task InitiatePayment_ServiceThrowsException_ShoulReturnErrorView([Frozen] PaymentRequestDto request)
        {
            // Arrange
            var controller = new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);
            _paymentsServiceMock.Setup(service => service.InitiatePaymentAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test Exception"));


            // Act
            var result = await controller.InitiatePayment(request, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Error");
        }

        [TestMethod]
        public async Task InitiatePayment_WithNullRequest_ShoulReturnErrorView()
        {
            // Act
            var controller = new GovPayFailureController(_paymentsServiceMock.Object, mockOptions.Object, _loggerMock.Object);
            var result = await controller.InitiatePayment(null, It.IsAny<CancellationToken>()) as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.ActionName.Should().Be("Index");
                result.ControllerName.Should().Be("Error");
            }
        }
    }
}
