using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {
        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenDashboardConfigurationIsNull(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns((DashboardConfiguration)null!);
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(true));

            // Act
            Action act = () => new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value cannot be null. (Parameter 'dashboardConfiguration')");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenRPDRootUrlServiceConfigurationIsNull(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(new DashboardConfiguration { RPDRootUrl = null!, SignOutUrl = new Service { Url = "signout" } });
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(true));

            // Act
            Action act = () => new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("dashboardConfiguration.Value.RPDRootUrl (Parameter 'dashboardConfiguration')");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenRPDRootUrlConfigurationIsNull(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(new DashboardConfiguration { RPDRootUrl = new Service { Url = null }, SignOutUrl = new Service { Url = "signout" } });
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(true));

            // Act
            Action act = () => new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("dashboardConfiguration.Value.RPDRootUrl (Parameter 'dashboardConfiguration')"); 
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenSignoutUrlConfigurationIsNull(
    [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
    [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(new DashboardConfiguration { RPDRootUrl = new Service { Url = "http://www.google.com" }, SignOutUrl = null! });
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(true));

            // Act
            Action act = () => new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("dashboardConfiguration.Value.SignOutUrl (Parameter 'dashboardConfiguration')");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldThrowArgumentNullException_WhenSignoutUrlServiceConfigurationIsNull(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(new DashboardConfiguration { RPDRootUrl = new Service { Url = "http://www.google.com" }, SignOutUrl = new Service { Url = null } });
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(true));

            // Act
            Action act = () => new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("dashboardConfiguration.Value.SignOutUrl (Parameter 'dashboardConfiguration')");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldReturnSignOutResult_WhenAuthenticationEnabled(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(new DashboardConfiguration { RPDRootUrl = new Service { Url = "http://www.google.com" }, SignOutUrl = new Service { Url = "signout" } });
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(true));

            AccountController controller = new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Act
            var result = controller.SignOut("Https://www.google.com");

            // Assert
            result.Should().BeOfType<SignOutResult>();
        }

        [TestMethod, AutoMoqData]
        public void Constructor_ShouldReturnOK_WhenAuthenticationDisabled(
            [Frozen] Mock<IOptions<DashboardConfiguration>> _dashboardConfigurationMock,
            [Frozen] Mock<IFeatureManager> featureManager)
        {
            // Arrange
            _dashboardConfigurationMock.Setup(x => x.Value).Returns(new DashboardConfiguration { RPDRootUrl = new Service { Url = "http://www.google.com" }, SignOutUrl = new Service { Url = "signout" } });
            featureManager.Setup(x => x.IsEnabledAsync("EnableAuthenticationFeature")).Returns(Task.FromResult(false));

            AccountController controller = new AccountController(_dashboardConfigurationMock.Object, featureManager.Object);

            // Act
            var result = controller.SignOut("");

            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
