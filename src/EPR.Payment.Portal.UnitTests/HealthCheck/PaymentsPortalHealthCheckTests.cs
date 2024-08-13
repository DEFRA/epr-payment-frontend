using AutoFixture;
using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.HealthCheck;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using System.Net;

namespace EPR.Payment.Portal.UnitTests.HealthCheck
{

    [TestClass]
    public class PaymentsPortalHealthCheckTests : HealthChecksTestsBase
    {
        [TestMethod, AutoMoqData]
        public async Task CheckHealthAsync_WhenServiceIsHealthy_ShouldReturnHealthy(
            [Frozen] Mock<IPaymentFacadeHealthService> _paymentFacadeHealthServiceMock,
            [Frozen] PaymentsPortalHealthCheck _healthCheck)
        {
            // Arrange
            _healthCheck = new PaymentsPortalHealthCheck(_paymentFacadeHealthServiceMock.Object);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            _paymentFacadeHealthServiceMock
                .Setup(service => service.GetHealthAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            var healthCheckContext = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(healthCheckContext);

            // Assert
            using(new AssertionScope())
            {
                result.Status.Should().Be(HealthStatus.Healthy);
                result.Description.Should().Be(PaymentsPortalHealthCheck.HealthCheckResultDescription);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task CheckHealthAsync_WhenServiceIsUnhealthy_ShouldReturnUnhealthy(
            [Frozen] Mock<IPaymentFacadeHealthService> _paymentFacadeHealthServiceMock,
            [Frozen] PaymentsPortalHealthCheck _healthCheck)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _healthCheck = new PaymentsPortalHealthCheck(_paymentFacadeHealthServiceMock.Object);
            _paymentFacadeHealthServiceMock
                .Setup(service => service.GetHealthAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            var healthCheckContext = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(healthCheckContext);

            // Assert
            using (new AssertionScope())
            {
                result.Status.Should().Be(HealthStatus.Unhealthy);
                result.Description.Should().Be(PaymentsPortalHealthCheck.HealthCheckResultDescription);
            }
        }

        [TestMethod, AutoMoqData]
        public async Task CheckHealthAsync_WhenServiceThrowsException_ShouldReturnUnhealthy(
            [Frozen] Mock<IPaymentFacadeHealthService> _paymentFacadeHealthServiceMock,
            [Frozen] PaymentsPortalHealthCheck _healthCheck)
        {
            // Arrange
            _paymentFacadeHealthServiceMock
                .Setup(service => service.GetHealthAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException());
            _healthCheck = new PaymentsPortalHealthCheck(_paymentFacadeHealthServiceMock.Object);

            var healthCheckContext = new HealthCheckContext();

            // Act
            var result = await _healthCheck.CheckHealthAsync(healthCheckContext);

            // Assert
            using (new AssertionScope())
            {
                result.Status.Should().Be(HealthStatus.Unhealthy);
                result.Description.Should().Be(PaymentsPortalHealthCheck.HealthCheckResultDescription);
            }
        }
    }
}
