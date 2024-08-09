using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Services;
using EPR.Payment.Portal.UnitTests.HealthCheck;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class PaymentFacadeHealthServiceTests : HealthChecksTestsBase
    {
        [TestMethod, AutoMoqData]
        public async Task GetHealthAsync_ValidQueryResult_ReturnsHttpStatusOK(
            [Frozen] Mock<IHttpPaymentFacadeHealthCheckService> httpPaymentFacadeHealthCheckService,
            PaymentFacadeHealthService paymentFacadeHealthService)
        {
            //Arrange
            httpPaymentFacadeHealthCheckService.Setup(x => x.GetHealthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResponseMessageOk);

            //Act
            var actual = await paymentFacadeHealthService.GetHealthAsync(CancellationToken.None);

            //Assert
            actual.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [TestMethod, AutoMoqData]
        public async Task GetHealthAsync_ValidQueryResult_ReturnsHttpStatusBasRequest(
            [Frozen] Mock<IHttpPaymentFacadeHealthCheckService> httpPaymentFacadeHealthCheckService,
            PaymentFacadeHealthService paymentFacadeHealthService)
        {
            //Arrange
            httpPaymentFacadeHealthCheckService.Setup(x => x.GetHealthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResponseMessageBadRequest);

            //Act
            var actual = await paymentFacadeHealthService.GetHealthAsync(CancellationToken.None);

            //Assert
            actual.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
