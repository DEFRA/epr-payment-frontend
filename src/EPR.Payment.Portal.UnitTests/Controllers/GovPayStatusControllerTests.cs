using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace GovPayStatusControllerTests
{
    [TestClass]
    public class GovPayStatusControllerTests
    {

        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException(
            [Frozen] Mock<IOptions<GovPayErrorConfiguration>> _mockGovPayErrorConfigurationOptions)
        {
            // Arrange
            _mockGovPayErrorConfigurationOptions.Setup(x => x.Value).Returns((GovPayErrorConfiguration)null!);

            // Act
            Action act = () => new GovPayStatusController(_mockGovPayErrorConfigurationOptions.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*govPayErrorConfiguration*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNotNull_ShouldInitialize(
            [Frozen] Mock<IOptions<GovPayErrorConfiguration>> _mockGovPayErrorConfigurationOptions)
        {
            // Act
            var controller = new GovPayStatusController(_mockGovPayErrorConfigurationOptions.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [TestMethod, AutoMoqData]
        public void Failure_WithCorrectConfiguration_ShouldReturnView(
            [Frozen] Mock<GovPayErrorConfiguration> _govPayErrorConfiguration,
            [Frozen] Mock<IOptions<GovPayErrorConfiguration>> _mockGovPayErrorConfigurationOptions)
        {
            // Arrange
            double paymentAmount = 10.0D;
            _mockGovPayErrorConfigurationOptions.Setup(o => o.Value).Returns(_govPayErrorConfiguration.Object);
            var controller = new GovPayStatusController(_mockGovPayErrorConfigurationOptions.Object);

            // Act
            var result = controller.Failure(paymentAmount) as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeOfType<ViewResult>();
                result!.Model.Should().Be(_mockGovPayErrorConfigurationOptions.Object);
            }

        }

        [TestMethod, AutoMoqData]
        public void Failure_WithGovPayErrorConfiguration_ShouldReturnViewResult_(
            [Frozen] Mock<IOptions<GovPayErrorConfiguration>> _mockGovPayErrorConfigurationOptions,
            [Frozen] double? paymentAmount)
        {
            // Arrange
            var _controller = new GovPayStatusController(_mockGovPayErrorConfigurationOptions.Object);

            // Act
            var result = _controller.Failure(paymentAmount) as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.Model.Should().BeEquivalentTo(_mockGovPayErrorConfigurationOptions.Object.Value);
                _controller.ViewData["PaymentAmount"].Should().Be(paymentAmount);
            }

        }

        [TestMethod, AutoMoqData]
        public void Failure_InViewData_ShouldSetPaymentAmount(
            [Frozen] Mock<IOptions<GovPayErrorConfiguration>> _mockGovPayErrorConfigurationOptions,
            [Frozen] double? paymentAmount)
        {
            // Arrange
            var _controller = new GovPayStatusController(_mockGovPayErrorConfigurationOptions.Object);

            // Act
            var result = _controller.Failure(paymentAmount) as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                _controller.ViewData["PaymentAmount"].Should().Be(paymentAmount);
            }

        }
    }
}