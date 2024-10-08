﻿using AutoFixture;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions.Execution;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class ErrorControllerTests
    {
        private Mock<DashboardConfiguration> mockDashboardConfig = null!;
        private Mock<IOptions<DashboardConfiguration>> mockOptions = null!;

        [TestInitialize]
        public void SetUp()
        {
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
            
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            mockOptions.Setup(o => o.Value).Returns((DashboardConfiguration)null!);
            Action act = () => new ErrorController(mockOptions.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*dashboardConfiguration*");
        }

        [TestMethod, AutoMoqData]
        public void Constructor_WhenConfigIsNotNull_ShouldInitialize()
        {
            // Act
            var controller = new ErrorController(mockOptions.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [TestMethod, AutoMoqData]
        public void Index_WithCorrectConfiguration_ShouldReturnView()
        {
            // Arrange
            var controller = new ErrorController(mockOptions.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeOfType<ViewResult>();
                result!.Model.Should().Be(mockDashboardConfig.Object);
            }

        }
    }
}
