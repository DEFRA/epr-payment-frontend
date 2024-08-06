using AutoFixture.MSTest;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.UnitTests.TestHelpers;
using EPR.Payment.Portal.Controllers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class GovPaySuccessControllerTests
    {
        private GovPaySuccessController _controller = null!;

        [TestInitialize]
        public void TestInitialize()
        {

            _controller = new GovPaySuccessController();
        }

        [TestMethod, AutoMoqData]
        public void Index_WithViewModel_ShouldReturnView([Frozen] CompletePaymentViewModel completePaymentResponseViewModel)
        {
            // Act
            var result = _controller.Index(completePaymentResponseViewModel) as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeOfType<ViewResult>();
            }

        }

        [TestMethod]
        public void Index_WithEmptyGuidId_ShoulReturnErrorView()
        {
            // Act
            var result = _controller.Index((CompletePaymentViewModel?)null) as RedirectToActionResult;

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
