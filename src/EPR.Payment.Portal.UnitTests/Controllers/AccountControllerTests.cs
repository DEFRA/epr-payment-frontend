using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenDashboardConfigurationIsNull(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns((DashboardConfiguration)null!);

            // Act
            Action act = () => new AccountController(_dashboardConfigurationMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'dashboardConfiguration')"); 
        }
    }
}
