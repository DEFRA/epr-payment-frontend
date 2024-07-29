using EPR.Payment.Portal.Common.Configuration;
using FluentAssertions;

namespace EPR.Payment.Portal.Common.UnitTests.DashboardConfigurationTests
{
    [TestClass]
    public class DashboardConfigurationTests
    {
        [TestMethod]
        public void SectionName_ShouldReturnDashboard()
        {
            // Act
            string sectionName = DashboardConfiguration.SectionName;

            // Assert
            sectionName.Should().Be("Dashboard");
        }
    }
}
