using EPR.Payment.Portal.Common.Configuration;
using FluentAssertions;

namespace EPR.Payment.Portal.Common.UnitTests.DashboardConfigurationTests
{
    [TestClass]
    public class GovPayStatusControllerTests
    {

        [TestMethod]
        public void SectionName_ShouldReturnDashboard()
        {
            // Act
            string sectionName = GovPayErrorConfiguration.SectionName;

            // Assert
            sectionName.Should().Be("GovPayError");
        }

    }
}
