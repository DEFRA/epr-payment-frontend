using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.HealthCheck;
using EPR.Payment.Portal.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace EPR.Payment.Portal.UnitTests.HealthCheck
{

    [TestClass]
    public class PaymentsPortalHealthCheckTests : HealthChecksTestsBase
    {
        [TestMethod, AutoMoqData]
        public async Task CheckHealthAsync_ValidQueryResult_ReturnsHealthyStatus(
        [Frozen] Mock<IPaymentFacadeHealthService> paymentServiceHealthService,
        HealthCheckContext healthCheckContext,
        PaymentsPortalHealthCheck paymentsPortalHealthCheck)
        {
            paymentServiceHealthService.Setup(x => x.GetHealthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResponseMessageOk);

            var actual = await paymentsPortalHealthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            actual.Status.Should().Be(HealthStatus.Healthy);
        }

        [TestMethod, AutoMoqData]
        public async Task CheckHealthAsync_NotValidQueryResult_ReturnsUnHealthyStatus(
            [Frozen] Mock<IPaymentFacadeHealthService> paymentServiceHealthService,
            HealthCheckContext healthCheckContext,
            PaymentsPortalHealthCheck paymentsPortalHealthCheck)
        {
            paymentServiceHealthService.Setup(x => x.GetHealthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResponseMessageBadRequest);

            var actual = await paymentsPortalHealthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            actual.Status.Should().Be(HealthStatus.Unhealthy);
        }
    }
}
